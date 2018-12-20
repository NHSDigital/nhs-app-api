package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageObject
import pages.HybridPageElement

@DefaultUrl("http://web.local.bitraft.io:3000/organDonation")
open class OrganDonationAdditionalDetailsPage : HybridPageObject() {

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
        religionSelector.assertIsVisible()
    }

    val additionalDetailsTitle = HybridPageElement(
            "//h2",
            null,
            null,
            this,
            "header").withText("Additional details")
}


