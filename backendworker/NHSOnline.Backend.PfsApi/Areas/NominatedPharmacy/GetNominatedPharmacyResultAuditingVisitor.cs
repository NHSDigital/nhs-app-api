using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.NominatedPharmacy.Models;

namespace NHSOnline.Backend.PfsApi.Areas.NominatedPharmacy
{
    public class GetNominatedPharmacyResultAuditingVisitor : IGetNominatedPharmacyResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<NominatedPharmacyController> _logger;

        private const string AuditType = AuditingOperations.GetNominatedPharmacyResponse;

        public GetNominatedPharmacyResultAuditingVisitor(IAuditor auditor, ILogger<NominatedPharmacyController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }

        public async Task Visit(GetNominatedPharmacyResult.Success result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Successfully retrieved nominated pharmacy");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetNominatedPharmacyResult.Success)}");
            }
        }

        public async Task Visit(GetNominatedPharmacyResult.PersonalChecksFailed result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Personal Details checks failed on the retrieved Nominated Pharmacy");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetNominatedPharmacyResult.PersonalChecksFailed)}");
            }
        }

        public async Task Visit(GetNominatedPharmacyResult.PharmacyChecksFailed result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Pharmacy checks failed on the retrieved Nominated Pharmacy");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetNominatedPharmacyResult.PharmacyChecksFailed)}");
            }
        }

        public async Task Visit(GetNominatedPharmacyResult.PharmacyRetrievalFailure result)
        {
            try
            {
                _logger.LogError("Error retrieving nominated pharmacy");
                await _auditor.PostOperationAudit(AuditType, "Failed to retrieve nominated pharmacy");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetNominatedPharmacyResult.PharmacyRetrievalFailure)}");
            }
        }

        public async Task Visit(GetNominatedPharmacyResult.NhsNumberSuperseded result)
        {
            try
            {
                _logger.LogInformation("The sent nhsNumber has been superseded - " +
                    $"old NhsNumber={result.SentNhsNumber}, new NhsNumber={result.ReturnedNhsNumber}");

                await _auditor.PostOperationAudit(AuditType, "The sent nhsNumber has been superseded - " +
                                                      $"old NhsNumber={result.SentNhsNumber}, new NhsNumber={result.ReturnedNhsNumber}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetNominatedPharmacyResult.NhsNumberSuperseded)}");
            }
        }

        public async Task Visit(GetNominatedPharmacyResult.ConfidentialAccount result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Account is marked as confidential");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetNominatedPharmacyResult.ConfidentialAccount)}");
            }
        }

        public async Task Visit(GetNominatedPharmacyResult.InternalServerError result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "An error occurred while trying to get the patient's nominated pharmacy");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetNominatedPharmacyResult.InternalServerError)}");
            }
        }

        public async Task Visit(GetNominatedPharmacyResult.ConfigNotEnabled result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Nominated pharmacy feature is disabled");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetNominatedPharmacyResult.ConfigNotEnabled)}");
            }
        }

        public async Task Visit(GetNominatedPharmacyResult.GpPracticeEpsNotEnabled result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, $"GP practice with ods code { result.OdsCode } " +
                                                      "not enabled for electronic prescription service");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetNominatedPharmacyResult.GpPracticeEpsNotEnabled)}");
            }
        }

        public async Task Visit(GetNominatedPharmacyResult.GpPracticeFailure result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, $"Error retrieving GP practice with ods code {result.OdsCode}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetNominatedPharmacyResult.GpPracticeFailure)}");
            }
        }

        public async Task Visit(GetNominatedPharmacyResult.NoNominatedPharmacy result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "No nominated pharmacy. Returning Success.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetNominatedPharmacyResult.NoNominatedPharmacy)}");
            }
        }

        public async Task Visit(GetNominatedPharmacyResult.PharmacyDetailFailure result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Error retrieving pharmacy using pharmacy OdsCde " +
                                                      $"{result.OdsCode} with status code {result.StatusCode}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetNominatedPharmacyResult.InvalidPharmacySubtype)}");
            }
        }

        public async Task Visit(GetNominatedPharmacyResult.InvalidPharmacySubtype result)
        {
            try
            {
                await _auditor.PostOperationAudit(AuditType, "Nominated pharmacy is disabled as user has invalid pharmacy subtype");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetNominatedPharmacyResult.InvalidPharmacySubtype)}");
            }
        }
    }
}
