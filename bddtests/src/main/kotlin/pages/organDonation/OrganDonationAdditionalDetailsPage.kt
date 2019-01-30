package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.sharedElements.DropdownElement

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
        ethnicitySelector.assertIsVisible()
        ethnicitySelector.assertSelected(defaultDropDownValue)
        religionSelector.assertIsVisible()
        ethnicitySelector.assertSelected(defaultDropDownValue)
        optionalInformationText.assertIsVisible()
    }

    private val optionalInformationText = HybridPageElement(
            "//p",
            page = this,
            helpfulName = "optional information text")
            .withText(
                    "This optional information is only used by NHSBT for analysis of the NHS Organ Donor Register " +
                            "and is not stored against your registration.")
}
