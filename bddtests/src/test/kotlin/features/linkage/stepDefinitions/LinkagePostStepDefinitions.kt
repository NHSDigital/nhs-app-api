package features.linkage.stepDefinitions

import constants.DateTimeFormats
import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.When
import features.linkage.LinkageResult
import mockingFacade.linkage.LinkageInformationFacade
import net.serenitybdd.core.Serenity
import org.joda.time.DateTime

open class LinkagePostStepDefinitions {

    @Given("I have valid (.*) linkage details for posting$")
    fun iHaveValidLinkageDetailsForPosting(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.SuccessfullyCreated)
    }

    @Given("I have valid (.*) linkage details apart from an empty email address$")
    fun iHaveValidLinkageDetailsExceptEmptyEmail(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier).copy(emailAddress = "")
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty surname$")
    fun validLinkageDetailsExceptEmptySurname(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier).copy(surname = "")
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkage)
    }

    @Given("I have valid (.*) linkage details apart from an empty date of birth$")
    fun iHaveValidLinkageDetailsExceptEmptyDateOfBirth(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier).copy(dateOfBirth = "")
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkage)
    }

    @Given("^I have valid (.*) linkage details for POST but I am under (\\d+)$")
    fun iAmUnderAgeForPost(gpSystem: String, age: Int) {
        val supplier = Supplier.valueOf(gpSystem)
        val now = DateTime.now()
        val dateOfBirth = now.minusYears(age).plusDays(1).withTime(0, 0, 0, 0)
        val linkage = LinkageFactory.validLinkage(supplier)
                .copy(dateOfBirth = dateOfBirth.toString(DateTimeFormats.backendDateTimeFormatWithoutTimezone))
        // don't need a gp supplier linkage result setup for the POST we do the age validation
        Serenity.setSessionVariable(LinkageInformationFacade::class).to(linkage)
    }

    @Given("^I have valid (.*) linkage details and try to create a linkage key as (\\d+) years old$")
    fun iAmXYearsOld(gpSystem: String, age: Int) {
        val supplier = Supplier.valueOf(gpSystem)
        val now = DateTime.now()
        val dateOfBirth = now.minusYears(age).withTime(0, 0, 0, 0)
        val linkageDateOfBirthFormat = LinkageFactory.getForSupplier(supplier).linkageDateOfBirthFormat
        val linkage = LinkageFactory.validLinkage(supplier)
                .copy(dateOfBirth = dateOfBirth.toString(linkageDateOfBirthFormat))
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.SuccessfullyCreated)
    }

    @Given("^I have valid (.*) linkage details and it's the first time a linkage key has been created for my " +
            "nhs number$")
    fun itsTheFirstTimeALinkageKeyHasBeenCreatedForMyNhsNumberAtThisPractice(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.SuccessfullyCreated)
    }

    @Given("I have valid (.*) linkage details but I already have an online account")
    fun iAlreadyHaveAnOnlineAccount(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.PatientAlreadyHasAnOnlineAccount)
    }

    @Given("I have valid (.*) linkage details but a linkage key already exists")
    fun iHaveValidLinkageDetailsButALinkageKeyAlreadyExists(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val linkage = LinkageFactory.validLinkage(supplier)
        LinkageFactory.setLinkageInformation(linkage, LinkageResult.LinkageKeyAlreadyExists)
    }

    @When("^I call the Linkage POST endpoint$")
    fun iCallTheLinkagePOSTEndpoint() {
        val linkage = LinkageFactory.setUpPostMocks()
        LinkageApi.post(linkage)
    }

    @When("^I call the Linkage POST endpoint which responds after (\\d+) seconds$")
    fun iCallTheLinkagePOSTEndpoint(delay: Long) {
        val linkage = LinkageFactory.setUpPostMocks(delay)
        LinkageApi.post(linkage)
    }

    @When("^I call the Linkage POST endpoint CID connection times out$")
    fun iCallTheLinkagePOSTEndpointCIDConnectionTimesOut() {
        val linkage = LinkageFactory.setUpPostMocks(DELAY)
        LinkageApi.post(linkage, DEFAULT_TIMEOUT_MILLISECONDS)
    }
}
