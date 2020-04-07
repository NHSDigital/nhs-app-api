package pages.organDonation

import mocking.data.organDonation.OrganDonationSerenityHelpers
import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.DropdownElement
import pages.sharedElements.expectedPage.ExpectedPageStructure
import utils.getOrFail

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationAdditionalDetailsPage : OrganDonationBasePage() {

    override val titleText: String = "Additional details"

    private val defaultDropDownValue = "Please select"

    val ethnicitySelector = DropdownElement(
            "Ethnicity",
            "Ethnicity Dropdown",
            this
    )

    val religionSelector = DropdownElement(
            "Religion",
            "Religion Dropdown",
            this
    )

    override fun assertDisplayed() {
        assertPageFullyLoaded()
        val expectedReligions = arrayListOf("Please select").plus(
                OrganDonationSerenityHelpers.REFERENCE_RELIGIONS.getOrFail<ArrayList<String>>())
        val expectedEthnicities = arrayListOf("Please select").plus(
                OrganDonationSerenityHelpers.REFERENCE_ETHNICITIES.getOrFail<ArrayList<String>>())
        val contents = ExpectedPageStructure()
                .h2("Additional details")
                .paragraph("This optional information is only used by the NHS to understand the make up of the " +
                        "NHS Organ Donor Register and is not stored against your registration.")
                .dropdown("Ethnicity (optional)", expectedEthnicities)
                .dropdown("Religion (optional)",expectedReligions)
                .button("Continue")
        contents.assert(this)
        ethnicitySelector.assertSelected(defaultDropDownValue)
        religionSelector.assertSelected(defaultDropDownValue)
    }
}
