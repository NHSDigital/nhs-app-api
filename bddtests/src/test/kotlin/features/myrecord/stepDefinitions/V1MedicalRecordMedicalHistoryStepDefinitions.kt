package features.myrecord.stepDefinitions

import constants.DateTimeFormats
import cucumber.api.java.en.Then
import features.myrecord.factories.MedicalHistoryFactory
import org.junit.Assert
import pages.myrecord.MedicalRecordV1Page
import utils.SerenityHelpers
import java.time.LocalDate
import java.time.format.DateTimeFormatter

open class V1MedicalRecordMedicalHistoryStepDefinitions : AbstractDemographicsStepDefinitions() {

    private lateinit var medicalRecordV1Page: MedicalRecordV1Page

    @Then("^I see the expected medical histories displayed - Medical Record v1$")
    fun thenISeeTheExpectedMedicalHistoriesDisplayedV1() {
        val expectedMedicalHistories = MedicalHistoryFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .getExpectedMedicalHistory()

        val onScreenMedicalHistories = medicalRecordV1Page.medicalHistories.allRecordItems()

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
