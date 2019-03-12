package pages.organDonation

import mocking.organDonation.models.KeyValuePair
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

    fun assertEthnicityAndReligion(ethnicity: String, religion:String) {
        OrganDonationDetailsAssertor.withH3Header(title, page).assertPair(
                arrayOf(
                        KeyValuePair("Ethnicity", ethnicity),
                        KeyValuePair("Religion", religion)))
    }
}