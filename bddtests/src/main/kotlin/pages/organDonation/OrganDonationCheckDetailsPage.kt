package pages.organDonation

import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.data.organDonation.isTrueOrFalse
import models.Patient
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.sharedElements.CheckBoxElement

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationCheckDetailsPage : OrganDonationBasePage() {

    override val titleText: String = "Check your details before submitting"

    val privacyStatementLink = HybridPageElement(
            "//a[normalize-space() = 'privacy statement']",
            page = this,
            helpfulName = "privacy statement link")

    val accuracyCheckBox = CheckBoxElement(this,
            "I confirm that the information given in this form is true, complete and accurate")

    val privacyStatementCheckBox = CheckBoxElement(this,
            "I have read the privacy statement and give consent for the use of my information in " +
                    "accordance with the terms")

    fun assertPersonalDetailsSection(patient: Patient) {
        OrganDonationDetailsAssertor("About you", this)
                .assertPair("Name", patient.formattedFullName())
                .assertPair("Date of birth", patient.formattedDateOfBirth())
                .assertPair("Gender", patient.sex.toString())
                .assertPair("NHS number", patient.formattedNHSNumber())
                .assert("The details above are retrieved from your GP services record, " +
                        "please contact your GP to amend them.")
    }

    val yourDecisionModule by lazy { OrganDonationYourDecisionModule(this) }
    val faithAndBeliefsModule by lazy { OrganDonationFaithModule(this) }
    val additionalDetailsModule by lazy { OrganDonationAdditionalDetailsModule(this) }

    override fun assertDisplayed() {
        assertPageFullyLoaded()
        assertConfirmationCheckBoxes()
    }

    private fun assertConfirmationCheckBoxes() {
        accuracyCheckBox.assertIsVisible()
        privacyStatementCheckBox.assertIsVisible()
    }


    fun clickSubmit() {
        val optIn = OrganDonationSerenityHelpers.IS_OPT_IN.isTrueOrFalse()
        clickOnButtonContainingText(if (optIn) "Yes I want to be a donor" else "No I do not want to be a donor")
    }
}