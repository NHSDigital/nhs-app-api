import cucumber.api.java.en.And
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.ExaminationsFactoryVision
import features.myrecord.stepDefinitions.AbstractDemographicsStepDefinitions
import org.junit.Assert
import org.junit.Assert.assertEquals

import pages.myrecord.MyRecordInfoPage


open class MyRecordExaminationsStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage
    lateinit var examinationsFactoryVision: ExaminationsFactoryVision


    @Then("^I see the examinations heading$")
    fun thenISeeTheExaminationsHeading() {
        val header = "Examinations"
        myRecordInfoPage.assertSectionHeaderIsVisible(header)
    }

    @And("^I see the examinations section collapsed$")
    fun andISeeTheExaminationsSectionCollapsed() {
        Assert.assertFalse(myRecordInfoPage.isExaminationsTextMsgVisible())
    }

    @And("^I do not have access to examinations$")
    fun givenIDoNotHaveAccessToExaminations() {
        setPatientToDefaultFor("VISION")
        examinationsFactoryVision = ExaminationsFactoryVision()
        examinationsFactoryVision.noAccess(patient)
    }

    @And("^the GP Practice has multiple examinations$")
    fun andTheGpPracticeHasMultipleExaminations() {
        setPatientToDefaultFor("VISION")
        examinationsFactoryVision = ExaminationsFactoryVision()
        examinationsFactoryVision.enabledWithRecords(patient)
    }

    @When("^I click the examinations section$")
    fun whenIClickTheExaminationsSection() {
        myRecordInfoPage.examinations.toggleShrub()
    }

    @And("^I have no examinations$")
    fun andIHaveNoExaminationsFor(heading: String) {
        assertTextInSection(heading, "No information recorded")
    }

    @And("^an error occurred retrieving the examinations")
    fun andAnErrorOccurredRetrievingTheDiagnosisFor() {
        setPatientToDefaultFor("VISION")
        examinationsFactoryVision = ExaminationsFactoryVision()
        examinationsFactoryVision.errorRetrieving(patient)
    }

    private fun assertTextInSection(heading: String, message: String) {
        val section = myRecordInfoPage.getSection(heading)
        section.header.assertSingleElementPresent()
        assertEquals(message, section.firstParagraph.text)
    }
}