package pages.organDonation

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertElementNotPresent

class OrganDonationAdditionalDetailsModule(private val page: HybridPageObject) {

    private val title = "Additional information"

    private val containerXPath = "//div[h2[text()=\"$title\"]]"

    private val container = HybridPageElement(
            containerXPath,
            page = page,
            helpfulName = "container for '$title' section")

    fun assertNotDisplayed(){
        container.assertElementNotPresent()
    }

    fun assertEthnicity(ethnicity: String) {
        OrganDonationDetailsAssertor.withH3Header(title, page)
                .assertPair("Ethnicity", ethnicity)
    }

    fun assertReligion(religion: String) {
        OrganDonationDetailsAssertor.withH3Header(title, page)
                .assertPair("Religion", religion)
    }
}