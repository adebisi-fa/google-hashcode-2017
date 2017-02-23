using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleHashCode
{
    public class Endpoint
    {
        public long Id { get; set; }
        public long TotalCacheServers { get; set; }
        public long LatencyFromDataCenter { get; set; }
    }

    public class CacheServerEndpointLatency
    {
        public long CacheServerId { get; set; }
        public long EndpointId { get; set; }
        public long Latency { get; set; }
    }

    public class VideoEndpointRequests
    {
        public long VideoId { get; set; }
        public long EndpointId { get; set; }
        public long TotalRequests { get; set; }

        public IEnumerable<VideoLatency> GetLatencies(Database db)
        {
            // Get latency for serving from the endpoints for this request
            var fromCacheServers = db.CacheServerEndpointLatencies.Where(
                e => e.EndpointId == EndpointId
            ).Select(
                e => new VideoLatency { CacheServerId = e.CacheServerId, Latency = e.Latency }
            );

            // Get latency for serving from datacenter
            var endpoint = db.Endpoints.FirstOrDefault(e => e.Id == EndpointId);
            return
                fromCacheServers.Union(new[]
                    {new VideoLatency {CacheServerId = null, Latency = endpoint.LatencyFromDataCenter}}
                );
        }
    }

    public class VideoLatency
    {
        public long? CacheServerId;
        public long Latency;
    }
}
