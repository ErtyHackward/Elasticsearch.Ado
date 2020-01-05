# Elasticsearch.Ado
ADO.NET wrapper for ElasticSearch

Elastic Search provides amazing X-Pack extensions that have native SQL support. We definetely must use it in .net!

## Usage

Install package from NuGet:

```
PM> Install-Package ElasticsearchAdo
```
Use in your code:
```C#
using Elasticsearch.Ado;
...
var connection = new ElasticSearchConnection();
connection.ConnectionString = "Server=localhost;Port=9200;User=guest;Password=guest;FetchSize=100000";
connection.Open();

var command = connection.CreateCommand();
command.CommandText = "SELECT bool_field, \"@timestamp\", long_field FROM \"my-elastic-index-*\" ORDER BY \"@timestamp\" DESC LIMIT 10";

using (var reader = command.ExecuteReader())
{
    while (reader.Read())
    {
        var boolField = reader.IsDBNull(0) ? "null" : reader.GetBoolean(0).ToString();
        var dtField = reader.IsDBNull(1) ? "null" : reader.GetDateTime(1).ToString();
        var longField = reader.IsDBNull(2) ? "null" : reader.GetInt64(2).ToString();

        Trace.WriteLine($"{boolField}, {dtField}, {longField}");
    }
}
```

## Limitations

* Elastic Search 6.3+
* Read-only access 
* Limited SQL functionality. No DISTINCT support yet. Please read [An Introduction to Elasticsearch SQL with Practical Examples](https://www.elastic.co/blog/an-introduction-to-elasticsearch-sql-with-practical-examples-part-1)
* [X-Pack SQL Limitations](https://www.elastic.co/guide/en/elasticsearch/reference/current/sql-limitations.html)
## Extending and deriving
If you've fixed some bugs or wrote some useful addition to this driver, please, do pull request them back here. 

If you need some functionality or found a bug but unable to implement/fix it, please file a ticket here, on GitHub.
