using System.Threading;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Support.Hasher;

namespace NHSOnline.Backend.Worker.UnitTests.Support.Hasher
{
    [TestClass]
    public class HashingServiceTests
    {
        private HashingService _hashService;

        [TestInitialize]
        public void TestInitialise()
        {
            _hashService = new HashingService();
        }

        [TestMethod]
        public void CheckHashIsConsistent()
        {
            const string key = "A000024176050002NxdUgCyDUAVz6";

            var e1 = _hashService.Hash(key);           
            Thread.Sleep(1000);
            var e2 = _hashService.Hash(key);
            
            e1.Should().BeEquivalentTo(e2);
        }
    }
}