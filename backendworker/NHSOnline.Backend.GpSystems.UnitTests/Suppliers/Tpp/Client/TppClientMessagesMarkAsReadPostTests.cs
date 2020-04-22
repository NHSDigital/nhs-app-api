using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientPracticeMessaging;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Client
{
    [TestClass]
    public class TppClientMessagesMarkAsReadPostTests
    {
        private TppRequestParameters _tppRequestParameters;
        private Mock<ITppClientRequestBuilder> _requestBuilder;
        private Mock<ITppClientRequestExecutor> _requestExecutor;

        private TppClientMessagesMarkAsReadPost _systemUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            _tppRequestParameters = new TppRequestParameters
            {
                OdsCode = "123",
                OnlineUserId = "456",
                PatientId = "789",
                Suid = "10"
            };

            _requestBuilder = new Mock<ITppClientRequestBuilder>();
            _requestExecutor = new Mock<ITppClientRequestExecutor>();

            _requestBuilder.Setup(b => b.Model(It.IsAny<MessagesMarkAsRead>()))
                .Returns(_requestBuilder.Object);

            _requestBuilder.Setup(b => b.Suid(It.IsAny<string>()))
                .Returns(_requestBuilder.Object);

            _requestExecutor.Setup(r => r.Post<MessagesMarkAsReadReply>(It.IsAny<Action<ITppClientRequestBuilder>>()))
                .Returns(Task.FromResult(new TppApiObjectResponse<MessagesMarkAsReadReply>(HttpStatusCode.OK)))
                .Callback<Action<ITppClientRequestBuilder>>(a => a(_requestBuilder.Object));

            _systemUnderTest = new TppClientMessagesMarkAsReadPost(_requestExecutor.Object);
        }

        [TestMethod]
        public async Task Post_WhenRequestParametersIsNull_ThenArgumentNullExceptionIsThrown()
        {
            await _systemUnderTest.Awaiting(r =>
                    r.Post((null, new List<string>())))
                .Should()
                .ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task Post_WhenMessageIdsListIsNull_ThenEmptyListIsAddedToRequest()
        {
            await _systemUnderTest.Post((_tppRequestParameters, null));

            _requestBuilder.Verify(b =>
                b.Model(It.Is<MessagesMarkAsRead>(r => r.Messages.Count == 0)));
        }

        [TestMethod]
        public async Task Post_WhenMessageIdsListIsEmpty_ThenEmptyListIsAddedToRequest()
        {
            await _systemUnderTest.Post((_tppRequestParameters, new List<string>()));

            _requestBuilder.Verify(b =>
                b.Model(It.Is<MessagesMarkAsRead>(r => r.Messages.Count == 0)));
        }

        [TestMethod]
        public async Task Post_WhenRequestParametersIsNotNull_ThenParametersAreAddedToRequest()
        {
            await _systemUnderTest.Post((_tppRequestParameters, new List<string>()));

            _requestBuilder.Verify(b =>
                b.Suid(It.Is<string>(r => r == _tppRequestParameters.Suid)));

            _requestBuilder.Verify(b =>
                b.Model(It.Is<MessagesMarkAsRead>(r =>
                    r.PatientId == _tppRequestParameters.PatientId
                        && r.OnlineUserId == _tppRequestParameters.OnlineUserId
                        && r.UnitId == _tppRequestParameters.OdsCode)));
        }

        [TestMethod]
        public async Task Post_WhenMessageIdsIsNotNullOrEmpty_ThenMessagesAreAddedToRequest()
        {
            var messageIds = new List<string> { "1", "2", "3" };

            await _systemUnderTest.Post((_tppRequestParameters, messageIds));

            _requestBuilder.Verify(b =>
                b.Model(It.Is<MessagesMarkAsRead>(r =>
                    r.Messages.Select(m => m.MessageId)
                        .Intersect(messageIds)
                        .ToList().Count == 3)));
        }
    }
}