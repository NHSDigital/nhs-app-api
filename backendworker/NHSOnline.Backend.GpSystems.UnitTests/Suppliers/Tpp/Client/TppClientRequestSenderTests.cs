using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models;
using RichardSzalay.MockHttp;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.Client
{
    [TestClass]
    public sealed class TppClientRequestSenderTests : IDisposable
    {
        private static readonly TimeSpan MaxWaitTime = TimeSpan.FromSeconds(10);

        private TppClientTestsContext Context { get; set; }

        private MockHttpMessageHandler MockHttpHandler => Context.MockHttpHandler;

        [TestInitialize]
        public void TestInitialize()
        {
            Context = new TppClientTestsContext();
            Context.Initialise();
        }

        [TestMethod]
        public async Task Post_ExecutesRequestInSeries_WhenMultipleConcurrentCallsAreMade()
        {
            // Arrange
            var requestOneMade = new ManualResetEventSlim();
            var requestOneContinue = new ManualResetEventSlim();

            var requestOne = new HttpRequestMessage(HttpMethod.Post, "http://tppapitest:60015/Test/One");
            MockHttpHandler.When(requestOne.Method, requestOne.RequestUri.ToString()).Respond(ProcessRequestOne);

            var requestTwo = new HttpRequestMessage(HttpMethod.Post, "http://tppapitest:60015/Test/Two");
            MockHttpHandler.When(requestOne.Method, requestTwo.RequestUri.ToString()).Respond(ProcessRequestTwo);

            var systemUnderTest = Context.ServiceProvider.GetRequiredService<ITppClientRequestSender>();

            // Act
            var callOne = Task.Run(() => systemUnderTest.SendRequestAndParseResponse<LogoffReply>(requestOne));
            requestOneMade.Wait(MaxWaitTime);
            var callTwo = Task.Run(() => systemUnderTest.SendRequestAndParseResponse<LogoffReply>(requestTwo));

            // Assert
            callTwo.Wait(TimeSpan.FromSeconds(2)).Should().BeFalse("second call should be blocked by first call");

            // Act
            requestOneContinue.Set();
            await callOne;
            await callTwo;

            Task<HttpResponseMessage> ProcessRequestOne()
            {
                requestOneMade.Set();
                requestOneContinue.Wait(MaxWaitTime);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            Task<HttpResponseMessage> ProcessRequestTwo() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        [TestMethod]
        public async Task Post_ExecutesRequestInSeries_WhenMultipleConcurrentCallsAreMadeToDifferentInstancesInTheSameScope()
        {
            // Arrange
            var requestOneMade = new ManualResetEventSlim();
            var requestOneContinue = new ManualResetEventSlim();

            var requestOne = new HttpRequestMessage(HttpMethod.Post, "http://tppapitest:60015/Test/One");
            MockHttpHandler.When(requestOne.Method, requestOne.RequestUri.ToString()).Respond(ProcessRequestOne);

            var requestTwo = new HttpRequestMessage(HttpMethod.Post, "http://tppapitest:60015/Test/Two");
            MockHttpHandler.When(requestOne.Method, requestTwo.RequestUri.ToString()).Respond(ProcessRequestTwo);

            using (var scope = Context.ServiceProvider.CreateScope())
            {
                var systemUnderTestOne = scope.ServiceProvider.GetRequiredService<ITppClientRequestSender>();
                var systemUnderTestTwo = scope.ServiceProvider.GetRequiredService<ITppClientRequestSender>();

                // Act
                var callOne = Task.Run(() => systemUnderTestOne.SendRequestAndParseResponse<LogoffReply>(requestOne));
                requestOneMade.Wait(MaxWaitTime);
                var callTwo = Task.Run(() => systemUnderTestTwo.SendRequestAndParseResponse<LogoffReply>(requestTwo));

                // Assert
                callTwo.Wait(TimeSpan.FromSeconds(2)).Should().BeFalse("second call should be blocked by first call");

                requestOneContinue.Set();
                await callOne;
                await callTwo;
            }

            Task<HttpResponseMessage> ProcessRequestOne()
            {
                requestOneMade.Set();
                requestOneContinue.Wait(MaxWaitTime);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            Task<HttpResponseMessage> ProcessRequestTwo() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        [TestMethod]
        public async Task Post_ExecutesRequestInParallel_WhenMultipleConcurrentCallsAreMadeToDifferentInstancesInDifferentScopes()
        {
            // Arrange
            var requestOneMade = new ManualResetEventSlim();
            var requestOneContinue = new ManualResetEventSlim();

            var requestOne = new HttpRequestMessage(HttpMethod.Post, "http://tppapitest:60015/Test/One");
            MockHttpHandler.When(requestOne.Method, requestOne.RequestUri.ToString()).Respond(ProcessRequestOne);

            var requestTwo = new HttpRequestMessage(HttpMethod.Post, "http://tppapitest:60015/Test/Two");
            MockHttpHandler.When(requestOne.Method, requestTwo.RequestUri.ToString()).Respond(ProcessRequestTwo);

            using (var scopeOne = Context.ServiceProvider.CreateScope())
            using (var scopeTwo = Context.ServiceProvider.CreateScope())
            {
                var systemUnderTestOne = scopeOne.ServiceProvider.GetRequiredService<ITppClientRequestSender>();
                var systemUnderTestTwo = scopeTwo.ServiceProvider.GetRequiredService<ITppClientRequestSender>();

                // Act
                var callOne = Task.Run(() => systemUnderTestOne.SendRequestAndParseResponse<LogoffReply>(requestOne));
                requestOneMade.Wait(MaxWaitTime);
                var callTwo = Task.Run(() => systemUnderTestTwo.SendRequestAndParseResponse<LogoffReply>(requestTwo));

                // Assert
                callTwo.Wait(TimeSpan.FromSeconds(2)).Should().BeTrue("second call should not be blocked by first call");

                requestOneContinue.Set();
                await callOne;
                await callTwo;
            }

            Task<HttpResponseMessage> ProcessRequestOne()
            {
                requestOneMade.Set();
                requestOneContinue.Wait(MaxWaitTime);
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            Task<HttpResponseMessage> ProcessRequestTwo() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        [TestCleanup]
        public void Dispose() => MockHttpHandler.Dispose();
    }
}