using System;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Elasticsearch.Ado
{
    public class ElasticSearchConnection : IDbConnection
    {
        private string _connectionString;
        private ElasticSearchConnectionString _connString;

        /// <summary>
        /// Example: "Server=127.0.0.1;Port=9200;User=admin;Password=123456;"
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                _connectionString = value;
                _connString = ElasticSearchConnectionString.Parse(_connectionString);
            }
        }

        /// <summary>
        /// This property don't have any influence because this is connection-less provider
        /// </summary>
        public int ConnectionTimeout { get; set; }

        /// <summary>
        /// Always returns null because there no "databases" in ElasticSearch
        /// </summary>
        public string Database => null;

        public ConnectionState State { get; private set; } = ConnectionState.Closed;


        /// <summary>
        /// Throws NotSupportedException
        /// </summary>
        /// <returns></returns>
        public IDbTransaction BeginTransaction()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Throws NotSupportedException
        /// </summary>
        /// <param name="il"></param>
        /// <returns></returns>
        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Does nothing because there no databases in elasticsearch
        /// </summary>
        public void ChangeDatabase(string databaseName)
        {
            
        }

        /// <summary>
        /// Does nothing because this is connection-less driver
        /// </summary>
        public void Close()
        {
            State = ConnectionState.Closed;
        }

        public ElasticSearchCommand CreateCommand()
        {
            return new ElasticSearchCommand(this);
        }

        IDbCommand IDbConnection.CreateCommand()
        {
            return CreateCommand();
        }

        /// <summary>
        /// Does nothing because this is connection-less driver
        /// </summary>
        public void Dispose()
        {
            State = ConnectionState.Closed;
        }

        public void Open()
        {
            var client = CreateClient();

            var result = client.PostAsync("", new StringContent("{ \"query\": \"SELECT 1\" }", Encoding.UTF8, "application/json")).Result;

            if (result.IsSuccessStatusCode)
                State = ConnectionState.Open;
            else
                State = ConnectionState.Closed;
        }

        internal HttpClient CreateClient()
        {
            var client = new HttpClient();

            if (!string.IsNullOrEmpty(_connString.User))
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Basic", Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                               $"{_connString.User}:{_connString.Password}")));

            client.BaseAddress = new Uri($"http://{_connString.Server}:{_connString.Port}/_sql?format=json");

            return client;
        }

    }
}
