using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.MetricLogFunctionApp.Slack
{
    public static class SlackServiceCollectionExtensions
    {
        internal static void AddSlack(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ISlackClient, SlackClient>();
            serviceCollection.AddConfig<SlackConfig>("Slack");
        }
    }
}