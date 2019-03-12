package pages.organDonation

import models.Patient
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.sharedElements.CheckBoxElement
import mocking.organDonation.models.KeyValuePair

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
        val bodyText = arrayOf("The details above are retrieved from your GP services record, " +
                "please contact your GP to amend them.")
        OrganDonationDetailsAssertor.withH3Header("About you", this)
                .assertPair(arrayOf(
                        KeyValuePair("Name", patient.formattedFullName()),
                        KeyValuePair("Date of birth", patient.formattedDateOfBirth()),
                        KeyValuePair("Gender", patient.sex.toString()),
                        KeyValuePair("NHS number", patient.formattedNHSNumber()),
                        KeyValuePair("Address", patient.address.full())))
                .assert(bodyText)
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
        clickOnButtonContainingText("Submit my decision")
    }
}