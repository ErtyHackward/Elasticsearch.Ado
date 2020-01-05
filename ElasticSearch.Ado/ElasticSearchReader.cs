using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Elasticsearch.Ado
{
    public class ElasticSearchReader : IDataReader
    {
        private ElasticSearchCommand _command;
        private ElasticSearchResponse _response;
        private int _line = -1;
        private ManualResetEvent _sync = new ManualResetEvent(false);

        public object this[int i] => GetValue(i);

        public object this[string name] => GetValue(GetOrdinal(name));

        public int Depth => 1;

        public bool IsClosed => _response == null;

        public int RecordsAffected => 0;

        public int FieldCount => _response == null ? 0 : _response.Columns.Count;

        internal ElasticSearchReader(ElasticSearchCommand command)
        {
            _command = command;
            MakeQuery();
        }

        internal async void MakeQuery()
        {
            _sync.Reset();
            _response = null;
            _line = -1;
                        
            try
            {
                var escapedCommand = _command.CommandText.Replace("\"", "\\\"");
                var response = await _command._httpClient.PostAsync("", new StringContent($"{{ \"query\": \"{escapedCommand}\" }}", Encoding.UTF8, "application/json")).ConfigureAwait(false);

                using (response)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    _response = JsonConvert.DeserializeObject<ElasticSearchResponse>(responseString);
                }
            }
            catch (Exception x)
            {

                throw;
            }
            finally
            {
                _sync.Set();
            }
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            _command = null;
            _response = null;
        }

        public bool GetBoolean(int i)
        {
            return (bool)_response.Rows[_line][i];
        }

        public byte GetByte(int i)
        {
            return (byte)_response.Rows[_line][i];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        public char GetChar(int i)
        {
            return (char)_response.Rows[_line][i];
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotSupportedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotSupportedException();
        }

        public string GetDataTypeName(int i)
        {
            return _response.Columns[i].Type;
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)_response.Rows[_line][i];
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)_response.Rows[_line][i];
        }

        public double GetDouble(int i)
        {
            return (double)_response.Rows[_line][i];
        }

        public Type GetFieldType(int i)
        {
            switch (_response.Columns[i].Type)
            {
                case "boolean": return typeof(bool);
                case "integer": return typeof(int);
                case "float": return typeof(float);
                case "byte": return typeof(byte);
                case "short": return typeof(short);
                case "long": return typeof(long);
                case "double": return typeof(double);
                case "half_float": return typeof(float);
                case "scaled_float": return typeof(double);
                case "keyword": return typeof(string);
                case "text": return typeof(string);
                case "binary": return typeof(byte[]);
                case "datetime": return typeof(DateTime);
                case "ip": return typeof(string);
                default: return typeof(object);
            }
        }

        public float GetFloat(int i)
        {
            return (float)_response.Rows[_line][i];
        }

        public Guid GetGuid(int i)
        {
            throw new NotSupportedException();
        }

        public short GetInt16(int i)
        {
            return (short)_response.Rows[_line][i];
        }

        public int GetInt32(int i)
        {
            return (int)_response.Rows[_line][i];
        }

        public long GetInt64(int i)
        {
            return (long)_response.Rows[_line][i];
        }

        public string GetName(int i)
        {
            return _response?.Columns[i].Name;
        }

        public int GetOrdinal(string name)
        {
            return _response.Columns.FindIndex(c => c.Name == name);
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            return (string)_response?.Rows[_line][i];
        }

        public object GetValue(int i)
        {
            return _response.Rows[_line][i];
        }

        public int GetValues(object[] values)
        {
            var count = Math.Min(_response.Columns.Count, values.Length);
            for (int i = 0; i < count; i++)
            {
                values[i] = GetValue(i);
            }
            return count;
        }

        public bool IsDBNull(int i)
        {
            return _response.Rows[_line][i] == null;
        }

        public bool NextResult()
        {
            return false;
        }

        public bool Read()
        {
            _sync.WaitOne();

            if (_response.Error != null)
                throw new DataException($"ElasticSearch request error: {_response.Error.Type} {_response.Error.Reason} {_response.Error.Unroll()}");

            return _response?.Rows.Count > ++_line;
        }
    }

    public class ElasticSearchResponse
    {
        [JsonProperty("columns")]
        public List<ElasticColumn> Columns { get; set; }

        [JsonProperty("rows")]
        public List<List<object>> Rows { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("error")]
        public ElasticError Error { get; set; }
    }

    public class ElasticColumn
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class ElasticError
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; }
        [JsonProperty("root_cause")]
        public List<ElasticError> RootCause { get; set; }

        public string Unroll()
        {
            if (RootCause?.Count > 0)
            {
                var sb = new StringBuilder();

                foreach (var item in RootCause)
                {
                    sb.Append(item.Type);
                    sb.Append(": ");
                    sb.Append(item.Reason);
                    sb.AppendLine();
                }

                return sb.ToString();
            }
            return string.Empty;
        }
    }
}
