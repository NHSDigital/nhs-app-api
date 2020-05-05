using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Metrics
{
    public interface IMetricContext
    {
        string NhsLoginId { get; }
        ProofLevel ProofLevel { get; }
        string OdsCode { get; }
    }
}