namespace Elasticsearch.Ado
{
    public struct ElasticSearchConnectionString
    {
        public string Server { get; set; }

        public string User { get; set; }

        public string Password { get; set; }

        public int Port { get; set; }

        public int FetchSize { get; set; }

        public static ElasticSearchConnectionString Parse(string connectionString)
        {
            // "Server=127.0.0.1;Port=9200;User=admin;Password=123456;"

            var str = new ElasticSearchConnectionString();

            str.Server = "127.0.0.1";
            str.Port = 9200;
            str.FetchSize = 1000;

            var spl = connectionString.Split(';');

            foreach (var item in spl)
            {
                var pos = item.IndexOf("=");
                if (pos == -1)
                    continue;

                var id = item.Substring(0, pos).Trim().ToLower();
                var value = item.Substring(pos + 1).Trim();

                switch (id)
                {
                    case "server": str.Server = value; break;
                    case "port": str.Port = int.Parse(value); break;
                    case "user": str.User = value; break;
                    case "password": str.Password = value; break;
                    case "fetchsize": str.FetchSize = int.Parse(value); break;
                    default: break;
                }
            }

            return str;
        }
    }
}
