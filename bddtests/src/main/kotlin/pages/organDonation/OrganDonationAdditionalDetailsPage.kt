package pages.organDonation

import mocking.data.organDonation.OrganDonationSerenityHelpers
import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.DropdownElement
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
        religionSelector.assertContents(expectedReligions)
        ethnicitySelector.assertContents(expectedEthnicities)
        ethnicitySelector.assertSelected(defaultDropDownValue)
        religionSelector.assertSelected(defaultDropDownValue)
    }
}
