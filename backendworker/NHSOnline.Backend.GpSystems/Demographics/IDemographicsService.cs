using System;
using NHSOnline.Backend.Support;
using System.Threading.Tasks;

namespace NHSOnline.Backend.GpSystems.Demographics
{
    public interface IDemographicsService
    {
        Task<DemographicsResult> GetDemographics(GpLinkedAccountModel gpLinkedAccountModel);
    }
}