package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.HybridPageElement
import pages.sharedElements.DropdownElement

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationAdditionalDetailsPage : HybridPageObject() {

    val additionalDetailsTitle = HybridPageElement(
            "//h2",
            "//h2",
            null,
            null,
            this,
            "header").withText("Additional details")

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

    fun assertIsDisplayed() {
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
