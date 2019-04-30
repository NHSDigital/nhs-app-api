namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{    
    internal interface IProcessState
    {   
        bool HasError { get; set; }
    }
}