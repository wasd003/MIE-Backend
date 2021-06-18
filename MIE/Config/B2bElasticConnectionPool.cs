using System;
using System.Linq;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;

namespace MIE
{
    public class B2bElasticConnectionPool : SniffingConnectionPool
    {
        public B2bElasticConnectionPool(IOptions<ElasticSearchConfig> options) : base(options.Value.Uris.Select(uri => new Uri(uri)))
        {
        }
    }
}
