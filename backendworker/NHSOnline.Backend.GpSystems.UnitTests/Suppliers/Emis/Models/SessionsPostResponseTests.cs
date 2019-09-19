using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.Models
{
    [TestClass]
    public class SessionsPostResponseTests
    {
        private const string SelfUpt = "weferfae";
        private const string SelfOdsCode = "dasdfsd";

        [TestMethod]
        public void UserPatientLinks_WithSelf_NoProxy()
        {
            //arrange
            var sessionPostResponse = new SessionsPostResponse
            {
                UserPatientLinks = new List<UserPatientLink>() { CreateSelfUserPatientLink() }
            };
            
            //assert
            Assert.AreEqual(SelfUpt, sessionPostResponse.ExtractUserPatientLinkToken() );
            Assert.IsFalse(sessionPostResponse.HasLinkedPatients());
        }

        [TestMethod]
        public void UserPatientLinks_WithSelf_AndProxy()
        {
            const string ProxyToken = "23423";
            const string ProxyOds = "678678";
            
            var proxyUser = new UserPatientLink()
            {
                UserPatientLinkToken = ProxyToken,
                AssociationType = AssociationType.Proxy,
                NationalPracticeCode = ProxyOds
            };

            var selfOriginal = CreateSelfUserPatientLink();
            
            //arrange
            var sessionPostResponse = new SessionsPostResponse
            {
                UserPatientLinks = new List<UserPatientLink>() { selfOriginal, proxyUser }
            };
            
            //assert
            Assert.AreEqual(SelfUpt, sessionPostResponse.ExtractUserPatientLinkToken() );
            Assert.IsTrue(sessionPostResponse.HasLinkedPatients());
            var proxies = sessionPostResponse.ExtractLinkedPatients();
            Assert.AreEqual(1, proxies.Count());
            Assert.AreEqual( proxyUser, proxies.FirstOrDefault()  );
            var self = sessionPostResponse.ExtractSelfPatient();
            Assert.AreEqual( selfOriginal, self  );
        }

        private UserPatientLink CreateSelfUserPatientLink()
        {
            return new UserPatientLink()
            {
                UserPatientLinkToken = SelfUpt,
                AssociationType = AssociationType.Self,
                NationalPracticeCode = SelfOdsCode
            };
        }

    }
}