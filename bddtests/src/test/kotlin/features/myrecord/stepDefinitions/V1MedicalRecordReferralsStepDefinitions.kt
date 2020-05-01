package features.myrecord.stepDefinitions

import constants.DateTimeFormats
import cucumber.api.java.en.Then
import features.myrecord.factories.ReferralsFactory
import org.junit.Assert
import pages.myrecord.MedicalRecordV1Page
import utils.SerenityHelpers
import java.time.LocalDate
import java.time.format.DateTimeFormatter

const val REFERRAL_DESCRIPTION_INDEX: Int = 0
const val SPECIALITY_INDEX: Int = 1
const val UBRN_INDEX: Int = 2

open class V1MedicalRecordReferralsStepDefinitions {

    private lateinit var medicalRecordV1Page: MedicalRecordV1Page

    @Then("^I see the expected referrals displayed - Medical Record v1$")
    fun thenISeeTheExpectedReferralsDisplayedV1() {
        val expectedReferrals = ReferralsFactory.getForSupplier(SerenityHelpers.getGpSupplier()).getExpectedReferrals()

        val onScreenReferrals = medicalRecordV1Page.referrals.allRecordItems()

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
            Assert.assertEquals(expectedReferrals[i].speciality, onScreenReferrals[i].bodyElements[SPECIALITY_INDEX])
            Assert.assertEquals(expectedReferrals[i].ubrn, onScreenReferrals[i].bodyElements[UBRN_INDEX])
        }
    }
}
