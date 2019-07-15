package features.nominatedPharmacy.steps

import com.github.tomakehurst.wiremock.stubbing.Scenario
import features.nominatedPharmacy.NominatedPharmacySerenityHelpers
import mocking.MockingClient
import mocking.data.nhsAzureSearchData.NhsAzureSearchData
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationReply
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationItem
import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationRequestBody
import mocking.spine.pds.GetNominatedPharmacyRequestBuilder
import mocking.spine.pds.PdsNominatedPharmacyBuilder
import mocking.spine.pds.PersonalCheckDetails
import utils.SerenityHelpers
import utils.set

open class NominatedPharmacyDataSetupSteps {

    private val mockingClient = MockingClient.instance

    fun setupNoNominatedPharmacy() {
        val nhsNumber = SerenityHelpers.getPatient().nhsNumbers[0]
        val surname = SerenityHelpers.getPatient().surname
        val dateOfBirth = SerenityHelpers.getPatient().dateOfBirthDigitsOnly()

        val responseStringForUpdatedPharmacy =
                GetNominatedPharmacyRequestBuilder.getResponse(nhsNumber, surname, dateOfBirth)

        mockingClient.forSpine {
            PdsNominatedPharmacyBuilder("urn:nhs:names:services:pdsquery/QUPA_IN000008UK02")
                    .respondWithSuccess(responseStringForUpdatedPharmacy)
                    .inScenario("changeNominatedPharmacy")
                    .whenScenarioStateIs(Scenario.STARTED)
        }

        val data = NhsAzureSearchData.generatePharmacyData(1)
        val generatedPharmacy = data.value[0]
        setupWiremockToReturnPharmacyWhenSearchedFor(generatedPharmacy)

        NominatedPharmacySerenityHelpers.MY_NOMINATED_PHARMACY.set(generatedPharmacy)
    }


    fun setupNominatedPharmacyWithInternetPharmacy(odsCode: String) {
        val nhsNumber = SerenityHelpers.getPatient().nhsNumbers[0]
        val surname = SerenityHelpers.getPatient().surname
        val dateOfBirth = SerenityHelpers.getPatient().dateOfBirthDigitsOnly()

        val personalDetails = PersonalCheckDetails(nhsNumber = nhsNumber, surname = surname, dateOfBirth = dateOfBirth)

        val responseStringForUpdatedPharmacy =
                GetNominatedPharmacyRequestBuilder.getResponse(personalDetails, odsCode, arrayOf("P1"))

        mockingClient.forSpine {
            PdsNominatedPharmacyBuilder("urn:nhs:names:services:pdsquery/QUPA_IN000008UK02")
                    .respondWithSuccess(responseStringForUpdatedPharmacy)
                    .inScenario("changeNominatedPharmacy")
                    .whenScenarioStateIs(Scenario.STARTED)
        }

        val data = NhsAzureSearchData.generatePharmacyData(1)
        val generatedPharmacy = data.value[0]
        generatedPharmacy.NACSCode = odsCode
        generatedPharmacy.OrganisationSubType = "Internet Pharmacy"
        setupWiremockToReturnPharmacyWhenSearchedFor(generatedPharmacy)

        NominatedPharmacySerenityHelpers.MY_NOMINATED_PHARMACY.set(generatedPharmacy)
    }

    fun setupNominatedPharmacyWithDifferentNhsNumber(pharmacyType: String, odsCode: String, nhsNumber : String) {
        val surname = SerenityHelpers.getPatient().surname
        val dateOfBirth = SerenityHelpers.getPatient().dateOfBirthDigitsOnly()

        val personalDetails = PersonalCheckDetails(nhsNumber = nhsNumber, surname = surname, dateOfBirth = dateOfBirth)

        val responseStringForUpdatedPharmacy = GetNominatedPharmacyRequestBuilder.
                getResponse(personalDetails, odsCode, arrayOf(pharmacyType))

        setupNominatedPharmacy(responseStringForUpdatedPharmacy, odsCode)
    }

    fun setupNominatedPharmacy(pharmacyType: String, odsCode: String) {
        val nhsNumber = SerenityHelpers.getPatient().nhsNumbers[0]
        val surname = SerenityHelpers.getPatient().surname
        val dateOfBirth = SerenityHelpers.getPatient().dateOfBirthDigitsOnly()

        val personalDetails = PersonalCheckDetails(nhsNumber = nhsNumber, surname = surname, dateOfBirth = dateOfBirth)

        val responseStringForUpdatedPharmacy = GetNominatedPharmacyRequestBuilder.getResponse(
                personalDetails, odsCode, arrayOf(pharmacyType))

        setupNominatedPharmacyWithResponseString(responseStringForUpdatedPharmacy, odsCode)
    }

    fun setupNominatedPharmacy(pharmacyType: String, odsCode: String, code: String? = null) {
        val  nhsNumber = SerenityHelpers.getPatient().nhsNumbers[0]
        val surname = SerenityHelpers.getPatient().surname
        val dateOfBirth = SerenityHelpers.getPatient().dateOfBirthDigitsOnly()

        val personalDetails = PersonalCheckDetails(nhsNumber = nhsNumber, surname = surname, dateOfBirth = dateOfBirth)

        val responseStringForUpdatedPharmacy = GetNominatedPharmacyRequestBuilder.
                getResponse(personalDetails, odsCode, arrayOf(pharmacyType), code)

        setupNominatedPharmacyWithResponseString(responseStringForUpdatedPharmacy, odsCode)
    }

    private fun setupNominatedPharmacyWithResponseString(responseStringForUpdatedPharmacy: String, odsCode: String) {
        mockingClient.forSpine {
            PdsNominatedPharmacyBuilder("urn:nhs:names:services:pdsquery/QUPA_IN000008UK02")
                    .respondWithSuccess(responseStringForUpdatedPharmacy)
                    .inScenario("changeNominatedPharmacy")
                    .whenScenarioStateIs(Scenario.STARTED)
        }

        val data = NhsAzureSearchData.generatePharmacyData(1)
        val generatedPharmacy = data.value[0]
        generatedPharmacy.NACSCode = odsCode
        setupWiremockToReturnPharmacyWhenSearchedFor(generatedPharmacy)

        NominatedPharmacySerenityHelpers.MY_NOMINATED_PHARMACY.set(generatedPharmacy)
    }

    fun setupWiremockForNominatedPharmacyWhenSpineFails() {
        mockingClient.forSpine {
            PdsNominatedPharmacyBuilder("urn:nhs:names:services:pdsquery/QUPA_IN000008UK02")
                    .respondWithServiceUnavailable()

        }
    }

    fun setupWiremockForNominatedPharmacyWhenAzureSearchFails() {
        val odsCode = SerenityHelpers.getPatient().odsCode

        mockingClient.forNhsAzureSearchOrganisation {
            nhsAzureSearch.nhsAzureSearchOrganisationRequest(NhsAzureSearchOrganisationRequestBody(
                    top = 1,
                    filter = "OrganisationTypeID eq 'GPB' and NACSCode eq '$odsCode'",
                    select = "OrganisationID,OrganisationName,NACSCode,Metrics",
                    search = null,
                    count = false,
                    searchFields = null,
                    searchMode = null,
                    queryType = null
            )).respondWithServiceUnavailable()
        }
    }

    fun setupWiremockForNominatedPharmacyPostUpdate(
            pharmacyType: String,
            organisation: NhsAzureSearchOrganisationItem) {

        val personalDetails = PersonalCheckDetails(
                nhsNumber =  SerenityHelpers.getPatient().nhsNumbers[0],
                surname = SerenityHelpers.getPatient().surname,
                dateOfBirth = SerenityHelpers.getPatient().dateOfBirthDigitsOnly())

        val responseStringForUpdatedPharmacy =
                GetNominatedPharmacyRequestBuilder.getResponse(
                        personalDetails,
                        organisation.NACSCode,
                        arrayOf(pharmacyType))

        mockingClient.forSpine {
            PdsNominatedPharmacyBuilder("urn:nhs:names:services:pds/PRPA_IN000203UK03")
                    .respondWithAccepted()
                    .inScenario("changeNominatedPharmacy")
                    .whenScenarioStateIs(Scenario.STARTED)
                    .willSetStateTo("pharmacyUpdated")
                    .also {
                        NominatedPharmacySerenityHelpers.PHARMACY_TO_BE_NOMINATED.set(null)
                        NominatedPharmacySerenityHelpers.MY_NOMINATED_PHARMACY.set(organisation)
                    }
        }

        mockingClient.forSpine {
            PdsNominatedPharmacyBuilder("urn:nhs:names:services:pdsquery/QUPA_IN000008UK02")
                    .respondWithSuccess(responseStringForUpdatedPharmacy)
                    .inScenario("changeNominatedPharmacy")
                    .whenScenarioStateIs("pharmacyUpdated")
        }

        setupWiremockToReturnPharmacyWhenSearchedFor(organisation)
    }

    private fun setupWiremockToReturnPharmacyWhenSearchedFor(organisation: NhsAzureSearchOrganisationItem) {
        mockingClient.forNhsAzureSearchOrganisation {
            nhsAzureSearch.nhsAzureSearchOrganisationRequest(NhsAzureSearchOrganisationRequestBody(
                    top = 1,
                    search = "*",
                    select = null,
                    searchFields = null,
                    filter = "NACSCode eq '${organisation.NACSCode}'",
                    queryType = null,
                    count = true
            )).respondWithSuccess(
                    NhsAzureSearchOrganisationReply(arrayListOf(organisation), 1)
            )
        }
    }

    fun enableGpPracticeForEPSForPatient() {
        val odsCode = SerenityHelpers.getPatient().odsCode
        val data = NhsAzureSearchData.generatePharmacyData(1)

        mockingClient.forNhsAzureSearchOrganisation {
            nhsAzureSearch.nhsAzureSearchOrganisationRequest(NhsAzureSearchOrganisationRequestBody(
                    top = 1,
                    filter = "OrganisationTypeID eq 'GPB' and NACSCode eq '$odsCode'",
                    select = "OrganisationID,OrganisationName,NACSCode,Metrics",
                    search = null,
                    count = false,
                    searchFields = null,
                    searchMode = null,
                    queryType = null
            )).respondWithSuccess(data)
        }
    }

    fun disableGpPracticeForEPSForPatient() {
        val odsCode = SerenityHelpers.getPatient().odsCode
        val data = NhsAzureSearchData.generatePharmacyData(1)
        data.value.get(0).Metrics = ""

        mockingClient.forNhsAzureSearchOrganisation {
            nhsAzureSearch.nhsAzureSearchOrganisationRequest(NhsAzureSearchOrganisationRequestBody(
                    top = 1,
                    filter = "OrganisationTypeID eq 'GPB' and NACSCode eq '${odsCode}'",
                    select = "OrganisationID,OrganisationName,NACSCode,Metrics",
                    search = null,
                    count = false,
                    searchFields = null,
                    searchMode = null,
                    queryType = null
            )).respondWithSuccess(data)
        }
    }

    fun setupWiremockForPharmacyTextSearch(searchText: String, data: NhsAzureSearchOrganisationReply) {
        mockingClient.forNhsAzureSearchOrganisation {
            nhsAzureSearch.nhsAzureSearchOrganisationRequest(NhsAzureSearchOrganisationRequestBody(
                    top = 10,
                    search = "Metrics:10051 AND ${searchText}*",
                    select = "OrganisationID,OrganisationName,Address1,Address2,Address3,City,Postcode," +
                            "NACSCode,Geocode,Contacts,OpeningTimes",
                    searchFields = "OrganisationName,Address2,Address3,City",
                    filter = "OrganisationSubType eq 'Community Pharmacy'",
                    queryType = "full",
                    searchMode = "all",
                    count = true
            )).respondWithSuccess(data)
        }
    }
}
