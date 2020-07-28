using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.PfsApi.GpSession;

namespace NHSOnline.Backend.PfsApi.UnitTests.GpSession
{
    [TestClass]
    public class LinkedAccountsGetIdsVisitorTests
    {
        [TestMethod]
        public void WhenVisitCalled_WithSuccess_ThenLinkedAccountIdsAreReturned()
        {
            var visitor = new LinkedAccountsGetIdsVisitor();

            var linkedAccounts = new List<LinkedAccount>
            {
                new LinkedAccount
                {
                    Id = Guid.NewGuid()
                },
                new LinkedAccount
                {
                    Id = Guid.NewGuid()
                }
            };

            Assert.IsTrue(
                (new List<Guid> { linkedAccounts[0].Id, linkedAccounts[1].Id }).SequenceEqual(
                    visitor.Visit(new LinkedAccountsResult.Success( linkedAccounts, false))));
        }
    }
}