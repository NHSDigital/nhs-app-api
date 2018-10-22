package features.linkage.stepDefinitions

import cucumber.api.java.en.*
import features.linkage.LinkageResult
import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.defaults.MockDefaults.Companion.DEFAULT_ODS_CODE
import mocking.defaults.MockDefaults.Companion.patient
import mocking.emis.EmisMappingBuilder
import mocking.emis.linkage.EmisLinkageGETBuilder
import mocking.emis.linkage.EmisLinkagePOSTBuilder
import mocking.emis.models.AddNhsUserRequest
import mocking.emis.models.AddNhsUserResponse
import mocking.emis.models.AddVerificationResponse
import net.serenitybdd.core.Serenity
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import mocking.emis.models.AddVerificationRequest
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.models.Mapping
import worker.models.linkage.CreateLinkageRequest
import worker.models.linkage.LinkageResponse

open class LinkageStepDefinitions {

    val HTTP_EXCEPTION = "HttpException"

    private val EMIS = "EMIS"
    lateinit var currentGPSystem: String
    lateinit var odsCode: String
    lateinit var nhsNumber: String
    private var accountId = "542343"
    private var identityToken = "abc"
    private var emailAddress = "ab@cd.com"

    private var linkageResult: LinkageResult = LinkageResult.SuccessfullyRetrieved
    private var linkageKey = "tTALtBP3rLR16"
    private var linkageResponse: LinkageResponse? = null

    val mockingClient = MockingClient.instance

    @Given("^I have a valid (.*) OdsCode$")
    fun ihaveAValidXOdsCode(gpSystem: String) {
        currentGPSystem = gpSystem

        when (currentGPSystem) {
            EMIS -> {
                odsCode = DEFAULT_ODS_CODE
            }
        }
    }

    @Given("^I have an empty (.*) OdsCode$")
    fun ihaveAnEmptyOdsCode(gpSystem: String) {
        currentGPSystem = gpSystem
        odsCode = ""
    }

    @Given("^I have a not found (.*) OdsCode$")
    fun ihaveANotFoundXOdsCode(gpSystem: String) {
        currentGPSystem = gpSystem
        odsCode = "A04889999"
    }

    @And("^I have a valid NhsNumber$")
    fun ihaveAnValidNhsNumber() {
        when (currentGPSystem) {
            EMIS -> {
                nhsNumber = "3434234345"
            }
        }
    }

    @And("^I have an empty NhsNumber$")
    fun ihaveAnEmptyNhsNumber() {
        nhsNumber = ""
    }

    @And("^I have a valid identity token$")
    fun ihaveAnValidIdentityToken() {
        when (currentGPSystem) {
            EMIS -> {
                identityToken = "abc"
            }
        }
    }

    @Given("^I have an empty identity token$")
    fun ihaveAnEmptyIdentityToken() {
        identityToken = ""
    }

    @And("^I have a valid email address$")
    fun ihaveAnValidEmailAddress() {
        emailAddress = "ab@cd.com"
    }

    @And("^I have an empty email address$")
    fun ihaveAnEmptyEmailAddress() {
        emailAddress = ""
    }

    @But("^The practice is not live$")
    fun thePraticeIsNotLive() {
        linkageResult = LinkageResult.PracticeNotLive
    }

    @But("^The GP system has marked me as archived$")
    fun theGpSystemHasMarkedMeAsArchived() {
        linkageResult = LinkageResult.PatientMarkedAsArchived
    }

    @But("^I am under 16$")
    fun iAmUnder16() {
        linkageResult = LinkageResult.PatientNonCompetentOrUnder16
    }

    @But("^My account status is invalid$")
    fun myAccountStatusIsInvalid() {
        linkageResult = LinkageResult.AccountStatusInvalid
    }

    @But("^I'm not registered at the practice$")
    fun imNotRegisteredAtThePractice() {
        linkageResult = LinkageResult.PatientNotRegisteredAtPractice
    }

    @But("^I am not found on the GP system$")
    fun iAmNotFoundOnTheGpSystem() {
        linkageResult = LinkageResult.NoRegisteredOnlineUserFound
    }

    @But("^The GP system responds with an internal server error retrieving the linkage key$")
    fun theGPSystemRespondsWithInternalServerErrorRetrievingLinkageKey() {
        linkageResult = LinkageResult.InternalServerError
    }

    @But("^The GP system responds with an internal server error creating the linkage key$")
    fun theGPSystemRespondsWithInternalServerErrorCreatingLinkageKey() {
        linkageResult = LinkageResult.InternalServerError
    }

    @And("^It's the first time a linkage key has been retrieved for an identity token$")
    fun itsTheFirstTimeALinkageKeyHasBeenRetrievedForAParticularIdentityToken() {
        linkageResult = LinkageResult.SuccessfullyRetrievedFirstTime
    }

    @And("^It's not the first time a linkage key has been retrieved for an identity token$")
    fun itsNotTheFirstTimeALinkageKeyHasBeenRetrievedForAParticularIdentityToken() {
        linkageResult = LinkageResult.SuccessfullyRetrieved
    }

    @And("^It's the first time a linkage key has been created for my nhs number$")
    fun itsTheFirstTimeALinkageKeyHasBeenCreatedForMyNhsNumberAtThisPractice() {
        linkageResult = LinkageResult.SuccessfullyCreated
    }

    @When("^I call the Linkage GET endpoint$")
    fun iCallTheLinkageGETEndpoint() {

        val linkageToGetRequestResponse = hashMapOf<LinkageResult, (EmisLinkageGETBuilder) -> Mapping>(
                LinkageResult.SuccessfullyRetrievedFirstTime to { get ->
                    get.respondWithSuccessfullyRetrievedFirstTime(AddVerificationResponse(odsCode, linkageKey, accountId))
                },
                LinkageResult.SuccessfullyRetrieved to { get ->
                    get.respondWithSuccessfullyRetrieved(AddVerificationResponse(odsCode, linkageKey, accountId))
                },
                LinkageResult.PatientNotRegisteredAtPractice to { get ->
                    get.respondWithPatientNotRegisteredAtPractice()
                },
                LinkageResult.NoRegisteredOnlineUserFound to { get -> get.respondWithNoRegisteredOnlineUserFound() },
                LinkageResult.PracticeNotLive to { get -> get.respondWithPracticeNotLive() },
                LinkageResult.PatientMarkedAsArchived to { get -> get.respondWithPatientMarkedAsArchived() },
                LinkageResult.PatientNonCompetentOrUnder16 to { get -> get.respondWithPatientNonCompetentOrUnder16() },
                LinkageResult.AccountStatusInvalid to { get -> get.respondWithAccountStatusInvalid() },
                LinkageResult.InternalServerError to { get -> get.respondWithInternalServerError() }
        )

        if (currentGPSystem == EMIS) {

            // end user session setup always required
            mockingClient.forEmis {
                authentication.endUserSessionRequest()
                        .respondWithSuccess(MockDefaults.DEFAULT_END_USER_SESSION_ID)
            }

            Assert.assertTrue("Test Setup Incorrect, Mapping not set up for linkage $linkageResult",
                    linkageToGetRequestResponse.containsKey(linkageResult))
            val response = linkageToGetRequestResponse[linkageResult]!!
            mockingClient.forEmis {
                response(authentication.linkageKeyGetRequest(AddVerificationRequest(nhsNumber, odsCode, identityToken)))
            }
        }

        try {
            linkageResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication.getLinkageKey(nhsNumber, odsCode, identityToken)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    private fun successfulPost(): (EmisLinkagePOSTBuilder) -> Mapping {

        mockingClient.forEmis {
            authentication.linkageKeyGetRequest(AddVerificationRequest(nhsNumber, odsCode, identityToken))
                    .respondWithSuccessfullyRetrievedFirstTime(AddVerificationResponse(odsCode, linkageKey, accountId))
        }

        return { post -> post.respondWithSuccessfullyCreated(AddNhsUserResponse(patient.connectionToken)) }
    }


    @When("I call the Linkage POST endpoint")
    fun iCallTheLinkagePOSTEndpoint() {
        val linkageToPostRequestResponse = hashMapOf(
                LinkageResult.SuccessfullyRetrieved to successfulPost(),
                LinkageResult.SuccessfullyCreated to successfulPost(),
                LinkageResult.PatientAlreadyHasAnOnlineAccount to { get -> get.respondWithPatientAlreadyHasAnOnlineAccount() },
                LinkageResult.NoRegisteredOnlineUserFound to { get -> get.respondWithNoRegisteredOnlineUserFound() },
                LinkageResult.PatientNotRegisteredAtPractice to null,
                LinkageResult.PracticeNotLive to { get -> get.respondWithPracticeNotLive() },
                LinkageResult.PatientMarkedAsArchived to { get -> get.respondWithPatientMarkedAsArchived() },
                LinkageResult.PatientNonCompetentOrUnder16 to { get -> get.respondWithPatientNonCompetentOrUnder16() },
                LinkageResult.InternalServerError to { get -> get.respondWithInternalServerError() }
        )

        if (currentGPSystem == EMIS) {
            // end user session setup always required
            mockingClient.forEmis {
                authentication.endUserSessionRequest()
                        .respondWithSuccess(MockDefaults.DEFAULT_END_USER_SESSION_ID)
            }
            Assert.assertTrue("Test Setup Incorrect, Mapping not set up for linkage $linkageResult",
                    linkageToPostRequestResponse.containsKey(linkageResult))
            val response = linkageToPostRequestResponse[linkageResult]
            if (response != null) {
                mockingClient.forEmis {
                    response(authentication.linkageKeyPOSTRequest(AddNhsUserRequest(odsCode, nhsNumber, emailAddress)))
                }
            }
        }
        try {
            linkageResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .authentication.postLinkageKey(CreateLinkageRequest(odsCode, nhsNumber, identityToken, emailAddress))
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @When("^I receive a valid response$")
    fun iReceiveAValidResponse() {
        Assert.assertNotNull(linkageResponse)
        Assert.assertEquals(odsCode, linkageResponse!!.odsCode)
        Assert.assertEquals(accountId, linkageResponse!!.accountId)
        Assert.assertEquals(linkageKey, linkageResponse!!.linkageKey)
    }

    @But("I already have an online account")
    fun iAlreadyHaveAnOnlineAccount() {
        linkageResult = LinkageResult.PatientAlreadyHasAnOnlineAccount
    }
}


