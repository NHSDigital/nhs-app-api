package pages.organDonation

import models.Patient
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject

@DefaultUrl("http://web.local.bitraft.io:3000/organDonation")
open class OrganDonationCheckDetailsPage : HybridPageObject() {

    val title = HybridPageElement(
            "//h2",
            page = this,
            helpfulName = "header").withText("Check your details before submitting")


    val privacyStatementLink = HybridPageElement(
            "//a[normalize-space() = 'privacy statement']",
            page = this,
            helpfulName = "privacy statement link")

    private val checkBoxXPath = "//div/input[@type='checkbox']/following-sibling::label"

    private val accuracyText = "I confirm that the information given in this form is true, complete and accurate"

    private val accuracyCheckBox = HybridPageElement(
            browserLocator = "$checkBoxXPath[normalize-space() = '$accuracyText']",
            androidLocator = null,
            page = this)

    private val privacyStatementCheckBoxText = "I have read the privacy statement and give consent for the use " +
            "of my information in accordance with the terms"

    private val privacyStatementCheckBox = HybridPageElement(
            browserLocator = "$checkBoxXPath[normalize-space() = '$privacyStatementCheckBoxText']",
            androidLocator = null,
            page = this)

    val yourDecisionModule by lazy { OrganDonationYourDecisionModule(this) }

    fun assertPersonalDetailsSection(patient: Patient) {
        OrganDonationDetailsAssertor("About you", this)
                .assertPair("Name", patient.formattedFullName())
                .assertPair("Date of birth", patient.formattedDateOfBirth())
                .assertPair("Gender", patient.sex.toString())
                .assertPair("NHS number", patient.formattedNHSNumber())
                .assert("The details above are retrieved from your GP services record, " +
                        "please contact your GP to amend them.")
    }

    fun assertConfirmationCheckBoxes() {
        accuracyCheckBox.assertSingleElementPresent().assertIsVisible()
        privacyStatementCheckBox.assertSingleElementPresent().assertIsVisible()
    }

    fun assertEthnicity(ethnicity: String) {
        OrganDonationDetailsAssertor("Additional information", this)
                .assertPair("Ethnicity", ethnicity)
    }

    fun assertReligion(religion: String) {
        OrganDonationDetailsAssertor("Additional information", this)
                .assertPair("Religion", religion)
    }

}


