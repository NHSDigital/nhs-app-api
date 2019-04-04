package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.DropdownElement
import pages.sharedElements.TextBlockElement

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
        TextBlockElement.withH2Header("Additional details", this)
                .assert("This optional information is only used by the NHS to understand the make up of the " +
                        "NHS Organ Donor Register and is not stored against your registration.")
        ethnicitySelector.assertIsVisible()
        ethnicitySelector.assertSelected(defaultDropDownValue)
        ethnicitySelector.assertSelected(defaultDropDownValue)
    }
}
