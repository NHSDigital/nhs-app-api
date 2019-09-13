package features.myrecord.stepDefinitions

import constants.DateTimeFormats
import cucumber.api.java.en.Then
import features.myrecord.factories.MedicalHistoryFactory
import org.junit.Assert
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers
import java.time.LocalDate
import java.time.format.DateTimeFormatter

open class MyRecordMedicalHistoryStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Then("^I see the expected medical histories displayed$")
    fun thenISeeTheExpectedMedicalHistoriesDisplayed() {
        val expectedMedicalHistories = MedicalHistoryFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .getExpectedMedicalHistory()

        val onScreenMedicalHistories = myRecordInfoPage.medicalHistories.allRecordItems()

        Assert.assertEquals(expectedMedicalHistories.count(), onScreenMedicalHistories.count())

        for (i in onScreenMedicalHistories.indices) {
            Assert.assertEquals(
                    LocalDate.parse(
                            expectedMedicalHistories[i].startDate.value),
                    LocalDate.parse(
                            onScreenMedicalHistories[i].label,
                            DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat)
                    ))
            Assert.assertEquals(expectedMedicalHistories[i].rubric.split(": ")[1],
                    onScreenMedicalHistories[i].bodyElements[0])
            Assert.assertEquals(expectedMedicalHistories[i].description.split(": ")[1],
                    onScreenMedicalHistories[i].bodyElements[1])
        }
    }
}
