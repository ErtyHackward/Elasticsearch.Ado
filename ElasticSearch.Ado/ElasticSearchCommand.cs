using System;
using System.Data;
using System.Net.Http;

namespace Elasticsearch.Ado
{
    public class ElasticSearchCommand : IDbCommand
    {
        internal HttpClient _httpClient;

        public string CommandText { get; set; }

        public int CommandTimeout { 
            get => (int)_httpClient.Timeout.TotalSeconds; 
            set => _httpClient.Timeout = TimeSpan.FromSeconds(value); 
        }

        public CommandType CommandType { get; set; }

        public IDbConnection Connection { get; set; }

        public ElasticSearchParameterCollection Parameters { get; } = new ElasticSearchParameterCollection();

        IDataParameterCollection IDbCommand.Parameters => Parameters;

        public IDbTransaction Transaction { get; set; }

        public UpdateRowSource UpdatedRowSource { get; set; }
        
        public ElasticSearchCommand(ElasticSearchConnection connection, string commandText = null)
        {
            Connection = connection;
            CommandText = commandText;

            _httpClient = connection.CreateClient();

        }

        public void Cancel()
        {
            throw new NotSupportedException();
        }

        public IDbDataParameter CreateParameter()
        {
            return new ElasticSearchParameter();
        }

        public void Dispose()
        {
            _httpClient = null;
            Connection = null;
        }

        public int ExecuteNonQuery()
        {
            throw new NotSupportedException();
        }

        public IDataReader ExecuteReader()
        {
            return new ElasticSearchReader(this);
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return ExecuteReader();
        }

        public object ExecuteScalar()
        {
            using (var reader = ExecuteReader())
            {
                if (reader.Read())
                    return reader.GetValue(0);
                return null;
            }
        }

        public void Prepare()
        {
            throw new NotSupportedException();
        }
    }
}
