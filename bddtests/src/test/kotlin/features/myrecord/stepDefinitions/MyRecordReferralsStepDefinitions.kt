package features.myrecord.stepDefinitions

import constants.DateTimeFormats
import cucumber.api.java.en.Then
import features.myrecord.factories.ReferralsFactory
import org.junit.Assert
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers
import java.time.LocalDate
import java.time.format.DateTimeFormatter

const val REFERRAL_DESCRIPTION_INDEX: Int = 0
const val SPECIAILITY_INDEX: Int = 1
const val UBRN_INDEX: Int = 2

open class MyRecordReferralsStepDefinitions : AbstractDemographicsStepDefinitions() {

    lateinit var myRecordInfoPage: MyRecordInfoPage

    @Then("^I see the expected referrals displayed$")
    fun thenISeeTheExpectedReferralsDisplayed() {
        val expectedReferrals = ReferralsFactory.getForSupplier(SerenityHelpers.getGpSupplier()).getExpectedReferrals()

        val onScreenReferrals = myRecordInfoPage.referrals.allRecordItems()

        Assert.assertEquals(expectedReferrals.count(), onScreenReferrals.count())

        for (i in onScreenReferrals.indices) {

            if (expectedReferrals[i].recordDate.value.equals("")) {
                Assert.assertEquals("Unknown Date", onScreenReferrals[i].label)
            } else {
                Assert.assertEquals(
                        LocalDate.parse(
                                expectedReferrals[i].recordDate.value),
                        LocalDate.parse(
                                onScreenReferrals[i].label,
                                DateTimeFormatter.ofPattern(DateTimeFormats.frontendBasicDateFormat)
                        ))
            }

            Assert.assertEquals(expectedReferrals[i].description,
                    onScreenReferrals[i].bodyElements[REFERRAL_DESCRIPTION_INDEX])
            Assert.assertEquals(expectedReferrals[i].speciality, onScreenReferrals[i].bodyElements[SPECIAILITY_INDEX])
            Assert.assertEquals(expectedReferrals[i].ubrn, onScreenReferrals[i].bodyElements[UBRN_INDEX])
        }
    }
}
