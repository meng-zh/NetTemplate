
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Dbers
{
    //StackExchange.Redis

    public class RedisConfig
    {
        public RedisConfig(string host,int port,string pwd)
        {
            Host = host;
            Port = port;
            Password = pwd;
        }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Password { get; set; }

    }

    public static class RedisExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, RedisConfig config)
        {
            var multiplexer = Create(config);
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            return services;
        }

        public static IConnectionMultiplexer Create(RedisConfig config)
        {
            var endPoints = new EndPointCollection();
            endPoints.Add(config.Host, config.Port);
            return ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                 Password = config.Password,
                 EndPoints = endPoints,
                 ConnectTimeout = (int)TimeSpan.FromSeconds(5000).TotalMilliseconds,
                 SyncTimeout = (int)TimeSpan.FromSeconds(5000).TotalMilliseconds
            });
        }

        public static void Close(this IConnectionMultiplexer multiplexer)
        {
            multiplexer.Close();
        }

        public static IDatabase SwitchDb(this IConnectionMultiplexer multiplexer, int db)
        {
            return multiplexer.GetDatabase(db);
        }
    }
}