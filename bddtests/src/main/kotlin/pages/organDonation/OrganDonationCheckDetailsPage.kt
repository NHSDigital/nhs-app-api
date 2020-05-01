package pages.organDonation

import models.Patient
import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.CheckBoxElement
import pages.sharedElements.expectedPage.ExpectedPageStructure

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationCheckDetailsPage : OrganDonationBasePage() {

    override val titleText: String = "About you"

    val accuracyCheckBox = CheckBoxElement(this,
            "I confirm that the information given in this form is true, complete and accurate")

    val privacyStatementCheckBox = CheckBoxElement(this,
            "I have read the privacy statement and give consent for the use of my information in " +
                    "accordance with the terms")

    fun assertPersonalDetailsSection(patient: Patient) {
        val bodyText = "Contact your GP surgery to amend your personal details."
        val expected = ExpectedPageStructure()
                .h3("Personal details")
                .h4("Name").paragraph(patient.formattedFullName())
                .h4("Date of birth").paragraph(patient.age.formattedDateOfBirth())
                .h4("Gender").paragraph(patient.sex.toString())
                .h4("NHS number").paragraph(patient.formattedNHSNumber())
                .paragraph(bodyText)
        expected.assert(this)
    }

    fun assertContactDetailsSection(patient: Patient) {
        ExpectedPageStructure()
                .h3("Contact details")
                .paragraph( "We will only contact you about your organ donation registration.")
                .h4("Postal address").paragraph(patient.contactDetails.address.full())
                .paragraph( "Contact your GP surgery to amend your postal address.")
                .assert(this)
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
