#pragma warning disable CS8618
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace LotusGoIMWebAPI.Common
{
    public class RedisHelper : IDisposable
    {
        private string connectionString;
        private string instanceName;
        private int defaultDb;
        private ConcurrentDictionary<string, ConnectionMultiplexer> connections;

        public RedisHelper(string connectionString, string instanceName, int defaultDb)
        {
            this.connectionString = connectionString;
            this.instanceName = instanceName;
            this.defaultDb = defaultDb;
            connections = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        }

        public ConnectionMultiplexer GetConnection()
        {
            return connections.GetOrAdd(instanceName, p=> ConnectionMultiplexer.Connect(connectionString));
        }

        public IDatabase GetDatabase()
        {
            return GetConnection().GetDatabase(defaultDb);
        }
        public IServer GetServer(string? configName = null, int endpointsIndex = 0)
        {
            var confOptions = ConfigurationOptions.Parse(connectionString);
            return GetConnection().GetServer(confOptions.EndPoints[endpointsIndex]);
        }
        public ISubscriber GetSubscriber()
        {
            return GetConnection().GetSubscriber();
        }

        public void Dispose()
        {
            if(connections != null && connections.Count > 0)
            {
                foreach(var item in connections.Values)
                {
                    item.Dispose();
                }
            }
        }
    }
}
