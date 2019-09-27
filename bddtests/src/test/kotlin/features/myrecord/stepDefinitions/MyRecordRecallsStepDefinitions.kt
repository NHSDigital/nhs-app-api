package features.myrecord.stepDefinitions

import constants.DateTimeFormats
import cucumber.api.java.en.Then
import features.myrecord.factories.RecallsFactory
import org.junit.Assert
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers
import java.time.LocalDate
import java.time.format.DateTimeFormatter

const val NAME_INDEX: Int = 0
const val DESC_INDEX: Int = 1
const val RESULT_INDEX: Int = 2
const val NEXT_DATE_INDEX: Int = 3
const val STATUS_INDEX: Int = 4

open class MyRecordRecallsStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Then("^I see the expected recalls displayed$")
    fun thenISeeTheExpectedRecallsDisplayed() {
        val expectedRecalls = RecallsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .getExpectedRecalls()

        val onScreenRecalls = myRecordInfoPage.recalls.allRecordItems()

        Assert.assertEquals(expectedRecalls.count(), onScreenRecalls.count())

        for (i in onScreenRecalls.indices) {

            if (expectedRecalls[i].recordDate.value.equals("")) {
                Assert.assertEquals("Unknown Date", onScreenRecalls[i].label)
            } else {
                Assert.assertEquals(
                        LocalDate.parse(
                                expectedRecalls[i].recordDate.value),
                        LocalDate.parse(
                                onScreenRecalls[i].label,
                                DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat)
                        ))
            }

            Assert.assertEquals(expectedRecalls[i].name, onScreenRecalls[i].bodyElements[NAME_INDEX])
            Assert.assertEquals(expectedRecalls[i].description, onScreenRecalls[i].bodyElements[DESC_INDEX])
            Assert.assertEquals(expectedRecalls[i].result, onScreenRecalls[i].bodyElements[RESULT_INDEX])
            Assert.assertEquals(expectedRecalls[i].nextDate, onScreenRecalls[i].bodyElements[NEXT_DATE_INDEX])
            Assert.assertEquals(expectedRecalls[i].status, onScreenRecalls[i].bodyElements[STATUS_INDEX])
        }
    }
}
