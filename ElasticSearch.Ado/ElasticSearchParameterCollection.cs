using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Elasticsearch.Ado
{
    public class ElasticSearchParameterCollection : List<ElasticSearchParameter>, IDataParameterCollection
    {
        public object this[string parameterName] { 
            get => this.First(p => p.ParameterName == parameterName).Value; 
            set => this.First(p => p.ParameterName == parameterName).Value = value; 
        }

        public bool Contains(string parameterName)
        {
            return this.Any(p => p.ParameterName == parameterName);
        }

        public int IndexOf(string parameterName)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i].ParameterName == parameterName)
                    return i;
            }
            return -1;
        }

        public void RemoveAt(string parameterName)
        {
            var index = IndexOf(parameterName);
            if (index != -1)
                RemoveAt(index);
        }
    }
}
