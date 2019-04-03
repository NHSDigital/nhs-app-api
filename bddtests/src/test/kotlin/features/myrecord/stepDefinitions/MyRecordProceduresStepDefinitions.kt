package features.myrecord.stepDefinitions

import config.Config
import cucumber.api.java.en.And
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.ProceduresFactoryVision
import features.sharedSteps.BrowserSteps
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import pages.myrecord.MyRecordInfoPage

class MyRecordProceduresStepDefinitions : AbstractDemographicsStepDefinitions() {

    @Steps
    lateinit var browser: BrowserSteps

    lateinit var myRecordInfoPage: MyRecordInfoPage
    lateinit var proceduresFactoryVision: ProceduresFactoryVision

    @And( "^I do not have access to procedures$" )
    fun givenIDoNotHaveAccessToProceduresFor(){
        setPatientToDefaultFor("VISION")
        proceduresFactoryVision = ProceduresFactoryVision()
        proceduresFactoryVision.noAccess(patient)
    }

    @And("^the GP Practice has multiple procedures$")
    fun andTheGpPracticeHasMultipleProceduresFor(){
        setPatientToDefaultFor("VISION")
        proceduresFactoryVision = ProceduresFactoryVision()
        proceduresFactoryVision.enabledWithRecords(patient)
    }

    @When("^I click the procedures section$" )
    fun whenIClickTheProceduresSection() {
        myRecordInfoPage.procedures.toggleShrub()
    }

    @And("^an error occurred retrieving the procedures")
    fun andAnErrorOccurredRetrievingTheProcedures() {
        setPatientToDefaultFor("VISION")
        proceduresFactoryVision = ProceduresFactoryVision()
        proceduresFactoryVision.errorRetrieving(patient)
    }

    @Then( "^I see procedures information$" )
    fun thenISeeProceduresInformation() {
        val sectionName = "Procedures"
        Assert.assertTrue(myRecordInfoPage.isVisionSectionPageVisible(sectionName, sectionName))
    }

    @When("^I enter url address for procedures detail directly into the url$")
    fun whenIEnterUrlAddressForProceduresDetailDirectlyIntoTheUrl() {
        val fullUrl = Config.instance.url + "/my-record/procedures-detail"
        browser.browseTo(fullUrl)
    }
}
