using System;
using System.Data;

namespace Elasticsearch.Ado
{
    public class ElasticSearchParameter : IDbDataParameter
    {
        public byte Precision { get; set; }
        public byte Scale { get; set; }
        public int Size { get; set; }
        public DbType DbType { get; set; }
        public ParameterDirection Direction { get => ParameterDirection.Input; set => throw new NotSupportedException(); }
        public bool IsNullable => false;
        public string ParameterName { get; set; }
        public string SourceColumn { get; set; }
        public DataRowVersion SourceVersion { get; set; }
        public object Value { get; set; }
    }
}
