package pages.organDonation

import pages.HybridPageObject
import pages.sharedElements.expectedPage.ExpectedPageStructure
import pages.sharedElements.expectedPage.ExpectedPageStructureAssertor

class OrganDonationAdditionalDetailsModule(private val page: HybridPageObject) {

    private val title = "Additional information"

    private val expected by lazy {
        ExpectedPageStructure().h3(title)
    }

    fun assertNotDisplayed() {
        ExpectedPageStructureAssertor().assertElementNotPresent(page, expected.build())
    }

    fun assertEthnicityAndReligion(ethnicity: String, religion: String) {
        val fullContent = expected
                .h4("Ethnicity")
                .paragraph(ethnicity)
                .h4("Religion")
                .paragraph(religion)
                .paragraph("This optional information is only used by the NHS to understand the make up of the NHS " +
                        "Organ Donor Register and is not stored against your registration.")

        ExpectedPageStructureAssertor().assert(page, fullContent.build())
    }

    companion object {
        const val didNotAnswer = "You did not answer"
    }
}