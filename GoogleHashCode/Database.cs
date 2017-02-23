using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleHashCode
{
    public class Database
    {
        public long TotalVideos;
        public long TotalEndpoints;
        public long TotalRequests;
        public long TotalCacheServers;
        public long CacheServerCapacity;
        public List<CacheServerEndpointLatency> CacheServerEndpointLatencies = new List<CacheServerEndpointLatency>();
        public List<VideoEndpointRequests> VideoEndpointRequests = new List<VideoEndpointRequests>();
        public List<Endpoint> Endpoints = new List<Endpoint>();
        public List<Video> Videos = new List<Video>();
        public Dictionary<long, IEnumerable<string>> GetOutput()
        {
            var groups = CacheServerEndpointLatencies.GroupBy(
                csel => csel.CacheServerId
            );

            return groups.ToDictionary(cache => cache.Key, cache =>
                {
                    return cache.SelectMany(eq => VideoEndpointRequests.Where(
                            r => r.EndpointId == eq.EndpointId && Videos.FirstOrDefault(c => c.Id == r.VideoId).Size <= CacheServerCapacity
                        ).Select(r => r.VideoId.ToString())
                    );
                }
            );
        }

        public static Database Build(string file)
        {
            var lines = File.ReadAllLines(file);

            var database = new Database();

            var summaryLine = lines[0].Split(' ');
            database.TotalVideos = summaryLine[0].ToLong();
            database.TotalEndpoints = summaryLine[1].ToLong();
            database.TotalRequests = summaryLine[2].ToLong();
            database.TotalCacheServers = summaryLine[3].ToLong();
            database.CacheServerCapacity = summaryLine[4].ToLong();

            database.Videos.AddRange(lines[1].Split(' ').Select((c, index) => new Video { Id = index, Size = c.ToLong() }));

            int currentLine = 2;
            string[] lineSplit;
            for (int i = 0; i < database.TotalEndpoints; i++)
            {
                lineSplit = lines[currentLine++].Split(' ');
                var endpoint = new Endpoint
                {
                    Id = i,
                    LatencyFromDataCenter = lineSplit[0].ToLong(),
                    TotalCacheServers = lineSplit[1].ToLong()
                };
                database.Endpoints.Add(endpoint);
                for (int j = 0; j < endpoint.TotalCacheServers; j++)
                {
                    lineSplit = lines[currentLine++].Split(' ');
                    database.CacheServerEndpointLatencies.Add(
                        new CacheServerEndpointLatency
                        {
                            CacheServerId = lineSplit[0].ToLong(),
                            EndpointId = endpoint.Id,
                            Latency = lineSplit[1].ToLong()
                        }
                    );
                }
            }

            for (int i = 0; i < database.TotalRequests; i++)
            {
                lineSplit = lines[currentLine++].Split(' ');
                database.VideoEndpointRequests.Add(new VideoEndpointRequests
                {
                    VideoId = lineSplit[0].ToLong(),
                    EndpointId = lineSplit[1].ToLong(),
                    TotalRequests = lineSplit[2].ToLong()
                });
            }

            return database;
        }
    }
}
