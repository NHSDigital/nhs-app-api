package pages.organDonation

import mocking.organDonation.models.KeyValuePair
import models.Patient
import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.CheckBoxElement
import pages.sharedElements.TextBlockElement

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationCheckDetailsPage : OrganDonationBasePage() {

    override val titleText: String = "About you"

    val privacyStatementLink = getLink("privacy statement")

    val accuracyCheckBox = CheckBoxElement(this,
            "I confirm that the information given in this form is true, complete and accurate")

    val privacyStatementCheckBox = CheckBoxElement(this,
            "I have read the privacy statement and give consent for the use of my information in " +
                    "accordance with the terms")

    fun assertPersonalDetailsSection(patient: Patient) {
        val bodyText = "Contact your GP surgery to amend your personal details."
        TextBlockElement.withH3Header("Personal details", this)
                .assertPair(arrayOf(
                        KeyValuePair("Name", patient.formattedFullName()),
                        KeyValuePair("Date of birth", patient.formattedDateOfBirth()),
                        KeyValuePair("Gender", patient.sex.toString()),
                        KeyValuePair("NHS number", patient.formattedNHSNumber())))
                .assert(bodyText)
    }

    fun assertContactDetailsSection(patient: Patient) {
        val bodyText1 = "We will only contact you about your organ donation registration."
        val bodyText2 = "Contact your GP surgery to amend your postal address."
        TextBlockElement.withH3Header("Contact details", this)
                .assert(bodyText1, bodyText2)
                .assertPair(arrayOf(
                        KeyValuePair("Postal address", patient.address.full())))
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