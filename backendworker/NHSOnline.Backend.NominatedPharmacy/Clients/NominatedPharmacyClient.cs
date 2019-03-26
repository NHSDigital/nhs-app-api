using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.Backend.NominatedPharmacy.ApiModels;
using NHSOnline.Backend.NominatedPharmacy.Clients.Interfaces;
using NHSOnline.Backend.NominatedPharmacy.Clients.Models;
using NHSOnline.Backend.NominatedPharmacy.Models;
using NHSOnline.Backend.NominatedPharmacy.ServiceDefinitions;
using NHSOnline.Backend.NominatedPharmacy.Soap;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.ResponseParsers;
using static NHSOnline.Backend.NominatedPharmacy.Soap.NominatedPharmacyTypes;

namespace NHSOnline.Backend.NominatedPharmacy.Clients
{
    public class NominatedPharmacyClient : INominatedPharmacyClient
    {
        private readonly INominatedPharmacyPDSClient _nominatedPharmacyPDSClient;
        private readonly INominatedPharmacySubmitClient _nominatedPharmacySubmitClient;

        public NominatedPharmacyClient(
            INominatedPharmacyPDSClient nominatedPharmacyPDSClient,
            INominatedPharmacySubmitClient nominatedPharmacySubmitClient)
        {
            _nominatedPharmacyPDSClient = nominatedPharmacyPDSClient;
            _nominatedPharmacySubmitClient = nominatedPharmacySubmitClient;
        }

        public async Task<NominatedPharmacyApiObjectResponse<QUPA_IN000009UK03_Response>> NominatedPharmacyGet(
            QUPA_IN000008UK02 getNominatedPharmacyRequest)
        {
            return await _nominatedPharmacyPDSClient.NominatedPharmacyGet(getNominatedPharmacyRequest);
        }

        public async Task<NominatedPharmacyApiObjectResponse<NominatedPharmacyUpdateResponse>> UpdateNominatedPharmacy(NominatedPharmacyUpdateRequest addNominatedPharmacyRequest)
        {
            return await _nominatedPharmacySubmitClient.UpdateNominatedPharmacy(addNominatedPharmacyRequest);
        }
    }
}