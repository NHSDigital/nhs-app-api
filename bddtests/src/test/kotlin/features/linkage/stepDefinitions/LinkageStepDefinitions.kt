package features.linkage.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import features.linkage.LinkageResult
import mockingFacade.linkage.LinkageInformationFacade
import net.serenitybdd.core.Serenity
import org.junit.Assert
import worker.models.linkage.LinkageResponse

const val DELAY: Long = 4
const val DEFAULT_TIMEOUT_MILLISECONDS: Int = 500

open class LinkageStepDefinitions {

    @Given("^another user has valid (.*) linkage details$")
    fun anotherUserHasValidLinkageDetailsFor(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validOtherLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.SuccessfullyRetrieved)
    }

    @Given("I have valid (.*) linkage details apart from a not found OdsCode$")
    fun iHaveValidLinkageDetailsExceptNotFoundOds(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier).copy(odsCode = "A04889999")
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty NhsNumber$")
    fun iHaveValidLinkageDetailsExceptEmptyNhsNumber(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier).copy(nhsNumber = "")
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty identity token$")
    fun iHaveValidLinkageDetailsExceptEmptyIdentityToken(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier).copy(identityToken = "")
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkage)
    }

    @Given("^I have valid (.*) linkage details but the practice is not live$")
    fun thePracticeIsNotLive(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.PracticeNotLive)
    }

    @Given("^I have valid (.*) linkage details but the GP system has marked me as archived$")
    fun theGpSystemHasMarkedMeAsArchived(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.PatientMarkedAsArchived)
    }

    @Given("^I have valid (.*) linkage details but there are multiple records for my NHS number$")
    fun thereAreMultipleRecordsForMyNhsNumber(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.MultipleRecordsFound)
    }

    @Given("^I have valid (.*) linkage details but I'm not registered at the practice$")
    fun imNotRegisteredAtThePractice(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.PatientNotRegisteredAtPractice)
    }

    @Given("^I have valid (.*) linkage details but the GP system responds with an internal server error " +
            "retrieving the linkage key$")
    fun theGPSystemRespondsWithInternalServerErrorRetrievingLinkageKey(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.InternalServerError)
    }

    @Given("^I have valid (.*) linkage details but the GP system responds with an internal server error " +
            "creating the linkage key$")
    fun theGPSystemRespondsWithInternalServerErrorCreatingLinkageKey(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.InternalServerError)
    }

    @Given("^another (.*) user has a new linkage key created for them$")
    fun anotherUserHasValidLinkageDetails(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validOtherLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.SuccessfullyCreated)
    }

    @Given("^I have valid (.*) linkage details but my nhs number is invalid$")
    fun iHaveValidLinkageDetailsButMyNhsNumberIsInvalid(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.InvalidNhsNumber)
    }

    @Given("^I have valid (.*) linkage details but my patient record was not found$")
    fun iHaveValidLinkageDetailsButMyPatientRecordWasNotFound(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.PatientRecordNotFound)
    }

    @Given("I have TPP linkage details which don't match in PDS$")
    fun iHaveTPPLinkageDetailsWhichDoNotMatchInPDS() {
        val linkage = LinkageFactory.validLinkage(Supplier.TPP)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.DetailsDoNotMatchInPDS)
    }

    @Given("I have incomplete or ended PFS registration details for a TPP user$")
    fun iHaveIncompleteOrEndedPFSRegistrationDetailsForATPPUser(){
        val linkage = LinkageFactory.validLinkage(Supplier.TPP)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.IncompletePFSRegistration)
    }

    @Given("I have TPP linkage details where the last name does not match SystmOne")
    fun iHaveTPPLinkageDetailsWhereTheLastNameDoesNotMatchSystmOne() {
        val linkage = LinkageFactory.validLinkage(Supplier.TPP)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.LastNameDoesNotMatch)
    }

    @Given("I have TPP linkage details where the DOB does not match SystmOne")
    fun iHaveTPPLinkageDetailsWhereTheDOBDoesNotMatchSystmOne() {
        val linkage = LinkageFactory.validLinkage(Supplier.TPP)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.DOBDoesNotMatch)
    }

    @Given("I have TPP linkage details where the user is not old enough")
    fun iHaveTPPLinkageDetailsWhereTheUserIsNotOldEnough() {
        val linkage = LinkageFactory.validLinkage(Supplier.TPP)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.PatientNotOldEnough)
    }

    @Given("I have TPP linkage details where no patient with the provided NHS number exists on SystmOne")
    fun iHaveTPPLinkageDetailsWhereNoPatientWithTheProvidedNHSNumberExistsOnSystmOne() {
        val linkage = LinkageFactory.validLinkage(Supplier.TPP)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.NoPatientWithNHSNumber)
    }

    @Given("I have TPP linkage details where the patient is not registered at the specified practice")
    fun iHaveTPPLinkageDetailsWhereThePatientIsNotRegisteredAtTheSpecifiedPractice() {
        val linkage = LinkageFactory.validLinkage(Supplier.TPP)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.PatientNotRegisteredAtSpecifiedPractice)
    }

    @Then("^I receive a valid linkage response$")
    fun iReceiveAValidResponse() {
        val linkageResponse = Serenity.sessionVariableCalled<LinkageResponse>(LinkageResponse::class)
        val linkage = Serenity.sessionVariableCalled<LinkageInformationFacade>(LinkageInformationFacade::class)

        Assert.assertNotNull(linkageResponse)
        Assert.assertEquals(linkage.odsCode, linkageResponse.odsCode)
        Assert.assertEquals(linkage.accountId, linkageResponse.accountId)
        Assert.assertEquals(linkage.linkageKey, linkageResponse.linkageKey)
    }
}
