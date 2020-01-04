using Elasticsearch.Ado;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace ElasticSearch.Ado.Tests
{
    [TestClass]
    public class UnitTest1
    {
        public string ConnectionString = "<write you connection string here>"; // example "Server=localhost;Port=9200;User=guest;Password=guest;"

        public UnitTest1()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        }

        [TestMethod]
        public void TestBasicConnection()
        {
            var connection = new ElasticSearchConnection();
            connection.ConnectionString = ConnectionString;
            connection.Open();
            Assert.IsTrue(connection.State == System.Data.ConnectionState.Open);
        }

        [TestMethod]
        public void TestReader()
        {
            var connection = new ElasticSearchConnection();
            connection.ConnectionString = ConnectionString;
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT action, mac FROM \"logstash-stats-apps-*\" LIMIT 10";

            Trace.WriteLine("Results:");
            int linesRead = 0;
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Trace.WriteLine($"{reader.GetString(0)}, {reader.GetString(1)}");
                    linesRead++;
                }
            }

            Assert.AreEqual(10, linesRead);
        }


        [TestMethod]
        public void TestFieldsMapping()
        {
            var connection = new ElasticSearchConnection();
            connection.ConnectionString = ConnectionString;
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT is_ott, \"@timestamp\" ts, dev_id FROM \"logstash-stats-apps-*\" ORDER BY \"@timestamp\" DESC LIMIT 10";

            Trace.WriteLine("Results:");
            int linesRead = 0;
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var boolField = reader.IsDBNull(0) ? "null" : reader.GetBoolean(0).ToString();
                    var dtField = reader.IsDBNull(1) ? "null" : reader.GetDateTime(1).ToString();
                    var longField = reader.IsDBNull(2) ? "null" : reader.GetInt64(2).ToString();

                    Trace.WriteLine($"{boolField}, {dtField}, {longField}");
                    linesRead++;
                }
            }

            Assert.AreEqual(10, linesRead);
        }

    }
}
