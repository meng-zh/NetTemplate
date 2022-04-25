using NUnit.Framework;
using StackExchange.Redis;

namespace Dbers.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ReadWrite()
        {
            var multiplexer = RedisExtension.Create(new RedisConfig("192.168.0.101", 6379, "HelloDev"));
            var db = multiplexer.GetDatabase(1);
            var key = new RedisKey("Dev:Test");
            var f1 = db.StringSet(key, "hello");
            Assert.IsTrue(f1);
            var res = db.StringGet(key);
            Assert.AreEqual("hello", res.ToString());
        }
    }
}