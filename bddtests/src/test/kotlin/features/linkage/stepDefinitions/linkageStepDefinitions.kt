package features.linkage.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.When
import features.linkage.LinkageResult
import mocking.MockingClient
import mocking.defaults.EmisMockDefaults
import mocking.defaults.MockDefaults
import mocking.defaults.TppMockDefaults
import mockingFacade.linkage.LinkageInformationFacade
import net.serenitybdd.core.Serenity
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.linkage.CreateLinkageRequest
import worker.models.linkage.LinkageResponse

open class LinkageStepDefinitions {

    val HTTP_EXCEPTION = "HttpException"

    val mockingClient = MockingClient.instance

    @Given("I have valid (.*) linkage details$")
    fun iHaveValidLinkageDetailsFor(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        setLinkageInformation(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty OdsCode$")
    fun iHaveValidLinkageDetailsExceptEmptyOds(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        linkage.odsCode = ""
        setLinkageInformation(linkage)
    }

    @Given("I have valid (.*) linkage details apart from a not found OdsCode$")
    fun iHaveValidLinkageDetailsExceptNotFoundOds(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        linkage.odsCode = "A04889999"
        setLinkageInformation(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty NhsNumber$")
    fun iHaveValidLinkageDetailsExceptEmptyNhsNumber(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        linkage.nhsNumber = ""
        setLinkageInformation(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty identity token$")
    fun iHaveValidLinkageDetailsExceptEmptyIdentityToken(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        linkage.identityToken = ""
        setLinkageInformation(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty email address$")
    fun iHaveValidLinkageDetailsExceptEmptyEmail(gpSystem: String) {
        val linkage = validLinkage(gpSystem)
        linkage.emailAddress = ""
        setLinkageInformation(linkage)
    }

    @Given("^The practice is not live$")
    fun thePraticeIsNotLive() {
        setLinkageResult(LinkageResult.PracticeNotLive)
    }

    @Given("^The GP system has marked me as archived$")
    fun theGpSystemHasMarkedMeAsArchived() {
        setLinkageResult(LinkageResult.PatientMarkedAsArchived)
    }

    @Given("^I am under 16$")
    fun iAmUnder16() {
        setLinkageResult(LinkageResult.PatientNonCompetentOrUnder16)
    }

    @Given("^My account status is invalid$")
    fun myAccountStatusIsInvalid() {
        setLinkageResult(LinkageResult.AccountStatusInvalid)
    }

    @Given("^I'm not registered at the practice$")
    fun imNotRegisteredAtThePractice() {
        setLinkageResult(LinkageResult.PatientNotRegisteredAtPractice)
    }

    @Given("^I am not found on the GP system$")
    fun iAmNotFoundOnTheGpSystem() {
        setLinkageResult(LinkageResult.NoRegisteredOnlineUserFound)
    }

    @Given("^The GP system responds with an internal server error retrieving the linkage key$")
    fun theGPSystemRespondsWithInternalServerErrorRetrievingLinkageKey() {
        setLinkageResult(LinkageResult.InternalServerError)
    }

    @Given("^The GP system responds with an internal server error creating the linkage key$")
    fun theGPSystemRespondsWithInternalServerErrorCreatingLinkageKey() {
        setLinkageResult(LinkageResult.InternalServerError)
    }

    @Given("^It's the first time a linkage key has been retrieved for an identity token$")
    fun itsTheFirstTimeALinkageKeyHasBeenRetrievedForAParticularIdentityToken() {
        setLinkageResult(LinkageResult.SuccessfullyRetrievedFirstTime)
    }

    @Given("^It's not the first time a linkage key has been retrieved for an identity token$")
    fun itsNotTheFirstTimeALinkageKeyHasBeenRetrievedForAParticularIdentityToken() {
        setLinkageResult(LinkageResult.SuccessfullyRetrieved)
    }

    @Given("^It's the first time a linkage key has been created for my nhs number$")
    fun itsTheFirstTimeALinkageKeyHasBeenCreatedForMyNhsNumberAtThisPractice() {
        setLinkageResult(LinkageResult.SuccessfullyCreated)
    }

    @Given("I already have an online account")
    fun iAlreadyHaveAnOnlineAccount() {
        setLinkageResult(LinkageResult.PatientAlreadyHasAnOnlineAccount)
    }

    @When("^I call the (.*) Linkage GET endpoint$")
    fun iCallTheLinkageGETEndpoint(gpSystem: String) {

        val linkageResult = Serenity.sessionVariableCalled<LinkageResult>(LinkageResult::class)
        val linkage = Serenity.sessionVariableCalled<LinkageInformationFacade>(LinkageInformationFacade::class)
        LinkageFactory.getForSupplier(gpSystem).mockLinkageGetResult(linkage, linkageResult)

        try {
            val linkageResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .authentication.getLinkageKey(
                            linkage.nhsNumber,
                            linkage.odsCode,
                            linkage.identityToken)

            Serenity.setSessionVariable(LinkageResponse::class).to(linkageResponse)

        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @When("I call the (.*) Linkage POST endpoint")
    fun iCallTheLinkagePOSTEndpoint(gpSystem: String) {

        val linkageResult = Serenity.sessionVariableCalled<LinkageResult>(LinkageResult::class)
        val linkage = Serenity.sessionVariableCalled<LinkageInformationFacade>(LinkageInformationFacade::class)
        LinkageFactory.getForSupplier(gpSystem).mockLinkagePostResult(linkage, linkageResult)

        try {
            val linkageResponse = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .authentication.postLinkageKey(CreateLinkageRequest(
                            linkage.odsCode,
                            linkage.nhsNumber,
                            linkage.identityToken,
                            linkage.emailAddress))

            Serenity.setSessionVariable(LinkageResponse::class).to(linkageResponse)

        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @When("^I receive a valid response$")
    fun iReceiveAValidResponse() {
        val linkageResponse = Serenity.sessionVariableCalled<LinkageResponse>(LinkageResponse::class)
        val linkage = Serenity.sessionVariableCalled<LinkageInformationFacade>(LinkageInformationFacade::class)

        Assert.assertNotNull(linkageResponse)
        Assert.assertEquals(linkage.odsCode, linkageResponse.odsCode)
        Assert.assertEquals(linkage.accountId, linkageResponse.accountId)
        Assert.assertEquals(linkage.linkageKey, linkageResponse.linkageKey)
    }

    private fun validLinkage(gpSystem: String): LinkageInformationFacade {
        val odsCode = when (gpSystem) {
            "EMIS" -> {
                EmisMockDefaults.DEFAULT_ODS_CODE_EMIS
            }
            "TPP" -> {
                TppMockDefaults.DEFAULT_ODS_CODE_TPP
            }
            else -> {
                Assert.fail("OdsCode not set up for $gpSystem")
                ""
            }
        }
        setLinkageResult(LinkageResult.SuccessfullyRetrieved)
        return LinkageInformationFacade(
                odsCode = odsCode,
                linkageKey = "tTALtBP3rLR16",
                accountId = "542343",
                nhsNumber = "3434234345",
                identityToken = "abc",
                emailAddress = "ab@cd.com")
    }

    private fun setLinkageInformation(linkageInformationFacade: LinkageInformationFacade) {
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkageInformationFacade)
    }

    private fun setLinkageResult(linkageResult: LinkageResult) {
        Serenity.setSessionVariable(LinkageResult::class).to(linkageResult)
    }
}


