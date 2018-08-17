package features.linkage.stepDefinitions

import cucumber.api.java.en.*
import features.linkage.LinkageResult
import mocking.MockingClient
import mocking.defaults.MockDefaults
import mocking.defaults.MockDefaults.Companion.DEFAULT_ODS_CODE
import mocking.emis.models.AddNhsUserRequest
import mocking.emis.models.AddNhsUserResponse
import mocking.emis.models.AddVerificationResponse
import net.serenitybdd.core.Serenity
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import mocking.emis.models.AddVerificationRequest
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
        when (currentGPSystem) {
            EMIS -> {

                // end user session setup always required
                mockingClient.forEmis {
                    endUserSessionRequest()
                            .respondWithSuccess(MockDefaults.DEFAULT_END_USER_SESSION_ID)
                }

                when (linkageResult) {

                    LinkageResult.SuccessfullyRetrievedFirstTime -> {
                        mockingClient.forEmis {
                            linkageKeyGetRequest(AddVerificationRequest(nhsNumber, odsCode, identityToken))
                                    .respondWithSuccessfullyRetrievedFirstTime(AddVerificationResponse(odsCode, linkageKey, accountId))
                        }
                    }

                    LinkageResult.SuccessfullyRetrieved -> {
                        mockingClient.forEmis {
                            linkageKeyGetRequest(AddVerificationRequest(nhsNumber, odsCode, identityToken))
                                    .respondWithSuccessfullyRetrieved(AddVerificationResponse(odsCode, linkageKey, accountId))
                        }
                    }

                    LinkageResult.PatientNotRegisteredAtPractice -> {
                        mockingClient.forEmis {
                            linkageKeyGetRequest(AddVerificationRequest(nhsNumber, odsCode, identityToken))
                                    .respondWithPatientNotRegisteredAtPractice()
                        }
                    }

                    LinkageResult.NoRegisteredOnlineUserFound -> {
                        mockingClient.forEmis {
                            linkageKeyGetRequest(AddVerificationRequest(nhsNumber, odsCode, identityToken))
                                    .respondWithNoRegisteredOnlineUserFound()
                        }
                    }

                    LinkageResult.PracticeNotLive -> {
                        mockingClient.forEmis {
                            linkageKeyGetRequest(AddVerificationRequest(nhsNumber, odsCode, identityToken))
                                    .respondWithPracticeNotLive()
                        }
                    }

                    LinkageResult.PatientMarkedAsArchived -> {
                        mockingClient.forEmis {
                            linkageKeyGetRequest(AddVerificationRequest(nhsNumber, odsCode, identityToken))
                                    .respondWithPatientMarkedAsArchived()
                        }
                    }

                    LinkageResult.PatientNonCompetentOrUnder16 -> {
                        mockingClient.forEmis {
                            linkageKeyGetRequest(AddVerificationRequest(nhsNumber, odsCode, identityToken))
                                    .respondWithPatientNonCompetentOrUnder16()
                        }
                    }

                    LinkageResult.AccountStatusInvalid -> {
                        mockingClient.forEmis {
                            linkageKeyGetRequest(AddVerificationRequest(nhsNumber, odsCode, identityToken))
                                    .respondWithAccountStatusInvalid()
                        }
                    }

                    LinkageResult.InternalServerError -> {
                        mockingClient.forEmis {
                            linkageKeyGetRequest(AddVerificationRequest(nhsNumber, odsCode, identityToken))
                                    .respondWithInternalServerError()
                        }
                    }
                }
            }
        }

        try {
            linkageResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getLinkageKey(nhsNumber, odsCode, identityToken)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @When("I call the Linkage POST endpoint")
    fun iCallTheLinkagePOSTEndpoint() {
        when (currentGPSystem) {
            EMIS -> {

                // end user session setup always required
                mockingClient.forEmis {
                    endUserSessionRequest()
                            .respondWithSuccess(MockDefaults.DEFAULT_END_USER_SESSION_ID)
                }

                when (linkageResult) {
                    LinkageResult.SuccessfullyCreated -> {
                        mockingClient.forEmis {
                            linkageKeyPOSTRequest(AddNhsUserRequest(odsCode, nhsNumber, emailAddress))
                                    .respondWithSuccessfullyCreated(AddNhsUserResponse(""))
                        }


                        mockingClient.forEmis {
                            linkageKeyGetRequest(AddVerificationRequest(nhsNumber, odsCode, identityToken))
                                    .respondWithSuccessfullyRetrievedFirstTime(AddVerificationResponse(odsCode, linkageKey, accountId))
                        }
                    }

                    LinkageResult.PatientAlreadyHasAnOnlineAccount -> {
                        mockingClient.forEmis {
                            linkageKeyPOSTRequest(AddNhsUserRequest(odsCode, nhsNumber, emailAddress))
                                    .respondWithPatientAlreadyHasAnOnlineAccount()
                        }
                    }

                    LinkageResult.NoRegisteredOnlineUserFound -> {
                        mockingClient.forEmis {
                            linkageKeyPOSTRequest(AddNhsUserRequest(odsCode, nhsNumber, emailAddress))
                                    .respondWithNoRegisteredOnlineUserFound()
                        }
                    }

                    LinkageResult.PracticeNotLive -> {
                        mockingClient.forEmis {
                            linkageKeyPOSTRequest(AddNhsUserRequest(odsCode, nhsNumber, emailAddress))
                                    .respondWithPracticeNotLive()
                        }
                    }

                    LinkageResult.PatientMarkedAsArchived -> {
                        mockingClient.forEmis {
                            linkageKeyPOSTRequest(AddNhsUserRequest(odsCode, nhsNumber, emailAddress))
                                    .respondWithPatientMarkedAsArchived()
                        }
                    }

                    LinkageResult.PatientNonCompetentOrUnder16 -> {
                        mockingClient.forEmis {
                            linkageKeyPOSTRequest(AddNhsUserRequest(odsCode, nhsNumber, emailAddress))
                                    .respondWithPatientNonCompetentOrUnder16()
                        }
                    }

                    LinkageResult.InternalServerError -> {
                        mockingClient.forEmis {
                            linkageKeyPOSTRequest(AddNhsUserRequest(odsCode, nhsNumber, emailAddress))
                                    .respondWithInternalServerError()
                        }
                    }
                }
            }
        }

        try {
            linkageResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .postLinkageKey(CreateLinkageRequest(odsCode, nhsNumber, identityToken, emailAddress))
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


