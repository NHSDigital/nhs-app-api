package features.myrecord.stepDefinitions

import constants.DateTimeFormats
import cucumber.api.java.en.Then
import features.myrecord.factories.EncountersFactory
import org.junit.Assert
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers
import java.time.LocalDate
import java.time.format.DateTimeFormatter

const val DESCRIPTION_INDEX: Int = 0
const val VALUE_INDEX: Int = 1
const val UNIT_INDEX: Int = 2

open class MyRecordEncountersStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Then("^I see the expected encounters displayed$")
    fun thenISeeTheExpectedEncountersDisplayed() {
        val expectedEncounters = EncountersFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .getExpectedEncounters()

        val onScreenEncounters = myRecordInfoPage.encounters.allRecordItems()

        Assert.assertEquals(expectedEncounters.count(), onScreenEncounters.count())

        for (i in onScreenEncounters.indices) {

            if (expectedEncounters[i].recordedOn.value.equals("")) {
                Assert.assertEquals("Unknown Date", onScreenEncounters[i].label)
            } else {
                Assert.assertEquals(
                        LocalDate.parse(
                                expectedEncounters[i].recordedOn.value),
                        LocalDate.parse(
                                onScreenEncounters[i].label,
                                DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat)
                        ))
            }

            Assert.assertEquals(expectedEncounters[i].description,
                    onScreenEncounters[i].bodyElements[DESCRIPTION_INDEX])
            Assert.assertEquals(expectedEncounters[i].value, onScreenEncounters[i].bodyElements[VALUE_INDEX])
            Assert.assertEquals(expectedEncounters[i].unit, onScreenEncounters[i].bodyElements[UNIT_INDEX])
        }
    }
}
