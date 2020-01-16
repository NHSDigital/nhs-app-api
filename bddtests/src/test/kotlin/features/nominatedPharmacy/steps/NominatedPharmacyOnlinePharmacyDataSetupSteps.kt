package features.nominatedPharmacy.steps


import mocking.MockingClient
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationReply
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationRequestBody

open class NominatedPharmacyOnlinePharmacyDataSetupSteps {

    private val mockingClient = MockingClient.instance

    fun setupWiremockForRandomisedOnlinePharmacies(data: NhsAzureSearchOrganisationReply)  {
        mockingClient.forAzure.forSearchOrganisation {

            nhsAzureSearch.nhsAzureSearchOrganisationRequest(NhsAzureSearchOrganisationRequestBody(
                    top = NhsAzureSearchData.RANDOMIZED_INTERNET_PHARMACY_LIMIT,
                    select = "OrganisationName,URL,Contacts,NACSCode",
                    filter  = "(OrganisationTypeID eq 'PHA') and (OrganisationSubType eq 'Internet Pharmacy')",
                    count  = true,
                    search = null,
                    searchFields = null,
                    queryType = null,
                    searchMode = null
            )).respondWithSuccess(data)
        }
    }

}
