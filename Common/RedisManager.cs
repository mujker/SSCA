using StackExchange.Redis;

namespace SSCA.Common
{
    public class RedisManager
    {
        public static ConnectionMultiplexer Redis;

        private RedisManager() { }
        static RedisManager()
        {
            if (Redis == null)
            {
                Redis = ConnectionMultiplexer.Connect(Settings.ConOption);
            }
        }

        public static bool SetRedisValue(RedisKey key, RedisValue value)
        {
            //如果连接中断尝试重连
            if (Redis.IsConnected == false)
            {
                Redis = ConnectionMultiplexer.Connect(Settings.ConOption);
            }
            return Redis.GetDatabase().StringSet(key, value);
        }
    }
}
