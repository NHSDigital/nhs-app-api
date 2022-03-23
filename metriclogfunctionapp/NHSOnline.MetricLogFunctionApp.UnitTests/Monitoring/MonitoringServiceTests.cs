using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.MetricLogFunctionApp.Monitoring.Models.AppInsightsAlert;
using SlackAPI;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Monitoring
{
    [TestClass]
    public class MonitoringServiceTests
    {
        [TestMethod]
        public async Task PostAlertReturnsCorrectMessageForUnknownAlert()
        {
            var context = new MonitoringServiceContext();
            var actualMessageBlock = new List<IBlock>();

            context.SlackClient.Setup(x => x.PostMessage(It.IsAny<IEnumerable<IBlock>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true))
                .Callback<IEnumerable<IBlock>, string>((messageBlocks, toAlertChannel) =>
                    actualMessageBlock = messageBlocks.ToList());
            await context.MonitoringService.PostAlert(CreateAppInsightAlert("Unknown Alert Rule"));

            var expected = new SlackMessageAssertor()
                .TextBlock(":rotating_light:   *Unknown Alert Rule*   :rotating_light:")
                .TextBlock(
                    "*UNKNOWN ALERT!*\nData as follows: \nunknownCol1 : TestValue1\nunknownCol2 : TestValue2\nunknownCol3 : TestValue3\n")
                .DividerBlock();

            expected.Assert(actualMessageBlock);
        }

        [DataTestMethod]
        [DataRow("AuditLog ETL Unprocessed Events Alert", DisplayName = "AuditLog ETL")]
        public async Task PostAlertReturnsCorrectMessageForUnprocessedEventsAlert(string alertText)
        {
            var context = new MonitoringServiceContext();
            var actualMessageBlock = new List<IBlock>();

            context.SlackClient.Setup(x => x.PostMessage(It.IsAny<IEnumerable<IBlock>>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true))
                .Callback<IEnumerable<IBlock>, string>((messageBlocks, toAlertChannel) =>
                    actualMessageBlock = messageBlocks.ToList());
            await context.MonitoringService.PostAlert(CreateAppInsightAlert(alertText));

            var expected = new SlackMessageAssertor()
                .TextBlock($":rotating_light:   *{alertText}*   :rotating_light:")
                .TextBlock(
                    "Environment : dev\nMessage : Failed() Encountered an error, this is a test\n" +
                    "Event Hub events details : PartionId: 0, Offset: 768809524160-768809524160, EnqueueTimeUtc: 2022-01-12T09:49:56.6850000+00:00-2022-01-12T09:49:56.6850000+00:00, SequenceNumber: 9200402-9200402, Count: 1\n")
                .DividerBlock()
                .TextBlock(
                    "Environment : dev-ci\nMessage : Failed() Encountered an error, this is a test ci\n" +
                    "Event Hub events details : PartionId: 1, Offset: 768809524160-768809524160, EnqueueTimeUtc: 2022-01-12T09:49:56.6850000+00:00-2022-01-12T09:49:56.6850000+00:00, SequenceNumber: 9200402-9200402, Count: 1\n")
                .DividerBlock();

            expected.Assert(actualMessageBlock);
        }

        private AppInsightAlert CreateAppInsightAlert(string alertRule)
        {
            var table = alertRule switch
            {
                "AuditLog ETL Unprocessed Events Alert" => CreateAppInsightsTableForUnprocessedEventAlert(),
                _ => CreateAppInsightsTableForUnknownAlert()
            };
            return new AppInsightAlert
            {
                Data = new AppInsightsData
                {
                    Essentials = new AppInsightsEssentials
                    {
                        AlertRule = alertRule
                    },
                    AlertContext = new AppInsightsAlertContext
                    {
                        SearchResults = new AppInsightsSearchResults
                        {
                            Tables = new List<AppInsightsTable>
                        {
                            table
                        }
                        }
                    }
                }
            };
        }

        private static AppInsightsTable CreateAppInsightsTableForUnprocessedEventAlert()
        {
            return new AppInsightsTable
            {
                Columns = new List<AppInsightColumn>
                {
                    new AppInsightColumn
                    {
                        Name = "env",
                        Type = "string"
                    },
                    new AppInsightColumn
                    {
                        Name = "message",
                        Type = "string"
                    },
                    new AppInsightColumn
                    {
                        Name = "eventhubevents",
                        Type = "string"
                    },

                },
                Rows = new List<List<object>>
                {
                    new List<object>
                    {
                        "dev",
                        "Failed() Encountered an error, this is a test",
                        "PartionId: 0, Offset: 768809524160-768809524160, EnqueueTimeUtc: 2022-01-12T09:49:56.6850000+00:00-2022-01-12T09:49:56.6850000+00:00, SequenceNumber: 9200402-9200402, Count: 1"
                    },
                    new List<object>
                    {
                        "dev-ci",
                        "Failed() Encountered an error, this is a test ci",
                        "PartionId: 1, Offset: 768809524160-768809524160, EnqueueTimeUtc: 2022-01-12T09:49:56.6850000+00:00-2022-01-12T09:49:56.6850000+00:00, SequenceNumber: 9200402-9200402, Count: 1"
                    }
                },
                Name = "AppInsightTable"
            };
        }

        private static AppInsightsTable CreateAppInsightsTableForUnknownAlert()
        {
            return new AppInsightsTable
            {
                Columns = new List<AppInsightColumn>
                {
                    new AppInsightColumn
                    {
                        Name = "unknownCol1",
                        Type = "string"
                    },
                    new AppInsightColumn
                    {
                        Name = "unknownCol2",
                        Type = "string"
                    },
                    new AppInsightColumn
                    {
                        Name = "unknownCol3",
                        Type = "real"
                    },

                },
                Rows = new List<List<object>>
                {
                    new List<object>
                    {
                        "TestValue1",
                        "TestValue2",
                        "TestValue3"
                    },
                },
                Name = "AppInsightTable"
            };
        }

    }
}

