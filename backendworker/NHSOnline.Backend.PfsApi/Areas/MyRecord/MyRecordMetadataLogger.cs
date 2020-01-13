using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.PatientRecord;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    public interface IMyRecordMetadataLogger
    {
        void LogMyRecordMetadata(UserSession userSession, GetMyRecordResult result);
    }

    public class MyRecordMetadataLogger : IMyRecordMetadataLogger
    {
        internal class RecordDetailMetaData
        {
            public string DetailName { get; }
            
            public bool? HasAccess { get; }
            
            public int ItemCount { get; }

            public RecordDetailMetaData(string detailName,  int itemCount, bool? hasAccess = null)
            {
                DetailName = detailName;
                HasAccess = hasAccess;
                ItemCount = itemCount;
            }
        }
        
        internal class MedicalRecordMetadata
        {
            public string Supplier { get; }
            
            public string OdsCode { get; }
            
            public List<RecordDetailMetaData> RecordDetailMetaData { get; }

            public MedicalRecordMetadata(string supplier, string odsCode, List<RecordDetailMetaData> recordDetailMetaData)
            {
                Supplier = supplier;
                OdsCode = odsCode;
                RecordDetailMetaData = recordDetailMetaData;
            }
        }
        
        private readonly ILogger<MyRecordMetadataLogger> _logger;
        private static readonly Supplier[] EnabledForSuppliers = { Supplier.Tpp, Supplier.Emis };

        public MyRecordMetadataLogger(
            ILogger<MyRecordMetadataLogger> logger
        )
        {
            _logger = logger;
        }

        public void LogMyRecordMetadata(UserSession userSession, GetMyRecordResult result)
        {
            if (userSession?.GpUserSession == null)
            {
                return;
            }

            if (!EnabledForSuppliers.Contains(userSession.GpUserSession.Supplier) || 
                !(result is GetMyRecordResult.Success successfulResult))
            {
                return;
            }
            
            var myRecordMetadata = BuildMyRecordMetadata(userSession, successfulResult);
            LogInformation(myRecordMetadata);
        }

        internal static MedicalRecordMetadata BuildMyRecordMetadata(UserSession userSession, 
            GetMyRecordResult.Success successfulResult)
        {
            Debug.Assert(successfulResult != null);

            var response = successfulResult.Response;

            var recordDetailMetaData = new List<RecordDetailMetaData>
            {
                BuildRecordDetailMetadata(response.Allergies, userSession, "Allergies"),
                BuildRecordDetailMetadata(response.Immunisations, userSession, "Immunisations",
                    Supplier.Emis),
                BuildRecordDetailMetadata(response.Medications, userSession, "Medications"),
                BuildRecordDetailMetadata(response.Problems, userSession, "Problems", 
                    Supplier.Emis),
                BuildRecordDetailMetadata(response.Consultations, userSession, "Consultations", 
                    Supplier.Emis),
                BuildRecordDetailMetadata(response.TppDcrEvents, userSession, "Tpp Dcr Events", 
                    Supplier.Tpp),
                BuildRecordDetailMetadata(response.TestResults, userSession, "Test Results"),
                BuildRecordDetailMetadata(response.Documents, userSession, "Documents")
            };
            
            var medicalRecordMetadata = new MedicalRecordMetadata(
                userSession.GpUserSession.Supplier.ToString(), 
                userSession.GpUserSession.OdsCode,
                recordDetailMetaData);

            return medicalRecordMetadata;
        }
        
        private static RecordDetailMetaData BuildRecordDetailMetadata(
            IPatientDataModel patientDataModel,
            UserSession userSession,
            string detailName,
            params Supplier[] applicableSuppliers
        )
        {
            if (applicableSuppliers.Any()
                && !applicableSuppliers.Contains(userSession.GpUserSession.Supplier))
            {
                return new RecordDetailMetaData(detailName, patientDataModel.RecordCount);
            }
            
            return new RecordDetailMetaData(
                detailName,
                patientDataModel.RecordCount,
                patientDataModel.HasAccess
            );
        }

        private void LogInformation(MedicalRecordMetadata myRecordMetadata)
        {
            Debug.Assert(myRecordMetadata!=null);
            _logger.LogInformation("medical_record_metadata=" + myRecordMetadata.SerializeJson());
        }
    }
}

