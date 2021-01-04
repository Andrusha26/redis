using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Redis.Child;
using Redis.Child.Infrastructure;

namespace Redis.Tests
{
    [TestClass]
    public class PartitionTest
    {
        private Partition _partition;

        [TestInitialize]
        public void Init()
        {
            var optionsMock = new Mock<IOptions<ChildOptions>>();
            optionsMock.Setup((o) => o.Value).Returns(() => new ChildOptions { PartitionItemsCount = 20000 });

            _partition = new Partition(optionsMock.Object);
        }

        [TestMethod]
        public async Task Partition_ConcurrentlyAdd_Success()
        {
            var firstTask = Task.Run(() =>
            {
                var c = 0;
                while (c < 10000)
                {
                    _partition.Add(c.ToString(), c.GetHashCode(), c);
                    c++;
                }
            });

            var secondTask = Task.Run(() =>
            {
                var c = 10001;
                while (c < 20000)
                {
                    _partition.Add(c.ToString(), c.GetHashCode(), c);
                    c++;
                }
            });

            await Task.WhenAll(firstTask, secondTask);

            var val = _partition.Get<int>(1.ToString(), 1.GetHashCode());

            Assert.AreEqual(val, 1);
        }

        [TestMethod]
        public void Partition_Add_ThrowOnDuplicatesWithSameHash()
        {
            var integer = 6357089;
            var charWithSameHash = 'a';

            _partition.Add(integer.ToString(), integer.GetHashCode(), integer);
            _partition.Add(charWithSameHash.ToString(), charWithSameHash.GetHashCode(), charWithSameHash);

            var intVal = _partition.Get<int>(integer.ToString(), integer.GetHashCode());
            var charVal = _partition.Get<char>(charWithSameHash.ToString(), charWithSameHash.GetHashCode());

            Assert.AreEqual(intVal, integer);
            Assert.AreEqual(charVal, charWithSameHash);
        }
    }
}
