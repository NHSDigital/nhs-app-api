package pages.organDonation

import mocking.organDonation.models.KeyValuePair
import pages.HybridPageObject
import pages.assertElementNotPresent

class OrganDonationAdditionalDetailsModule(private val page: HybridPageObject) {

    private val title = "Additional information"

    private val assertor by lazy {
        OrganDonationDetailsAssertor.withH3Header(title, page)
    }

    fun assertNotDisplayed() {
        assertor.assertElementNotPresent()
    }

    fun assertEthnicityAndReligion(ethnicity: String, religion: String) {
        assertor.assert("This optional information is only used by the NHS to understand the make up of the NHS " +
                "Organ Donor Register and is not stored against your registration.")
                .assertPair(
                        arrayOf(
                                KeyValuePair("Ethnicity", ethnicity),
                                KeyValuePair("Religion", religion)))
    }

    companion object {
        const val didNotAnswer = "You did not answer"
    }
}