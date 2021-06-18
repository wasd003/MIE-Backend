using System;
using Elasticsearch.Net;
using Nest;

namespace MIE.Config
{
    public class B2bElasticClient : ElasticClient
    {
        public B2bElasticClient(IConnectionPool pool) : base(new ConnectionSettings(pool))
        {
        }
    }
}
