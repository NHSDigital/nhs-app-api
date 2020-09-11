package pages.organDonation

import mocking.data.organDonation.OrganDonationSerenityHelpers
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.sharedElements.expectedPage.ExpectedPageStructure
import pages.sharedElements.expectedPage.ExpectedPageStructureAssertor
import pages.sharedElements.expectedPage.ParsedPage
import utils.isTrueOrFalse

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
class OrganDonationChoicePage : OrganDonationBasePage() {

    private val amendTitle = "Change your decision"
    private val registerTitle = "Register your decision"
    override val titleText: String by lazy { getCorrectTitleText() }

    val noButton = button("NO", "I do not want to donate my organs")

    val yesButton = button("YES", "I want to donate all or some of my organs")

    private fun button(option: String, description: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "//button[descendant::*[contains(text(),\"$option\")]]" +
                        "//p[contains(text(),'$description')]",
                webMobileLocator = "//button[descendant::*[contains(text(),\"$option\")]]" +
                        "//p[contains(text(),'$description')]",
                androidLocator = null,
                page = this,
                helpfulName = "$option option"
        )
    }

    override fun assertDisplayed() {
        //Please do not delete until NHSO-8407 and NHSO-8408 are completed
        assertDisplayed(OrganDonationSerenityHelpers.IS_AMEND_JOURNEY.isTrueOrFalse())
    }

    fun assertDisplayed(amend : Boolean) {
        val title = if (amend) amendTitle else registerTitle
        val expected = if (amend) createAmendExpectedPage(title) else createRegistrationExpectedPage(title)
        val page = ParsedPage.parse(this, "//div[div/h2[normalize-space(text())='${title}']]")
        ExpectedPageStructureAssertor().assert(page, expected.build())
    }

    private fun createRegistrationExpectedPage(title: String): ExpectedPageStructure{
        val expected = ExpectedPageStructure()
                .menuLinks(listOf("Find out more about organ donation"))
                .inset {
                    paragraph("If you have not registered your decision, " +
                            "changes to the law around organ donation may affect you.")
                }
                .h2(title)
        addButtonsToExpectedPage(expected)
        expected.menuLinks(listOf("Think you have registered already?"))
        return expected
    }

    private fun createAmendExpectedPage(title: String): ExpectedPageStructure{
        val expected = ExpectedPageStructure()
                .menuLinks(listOf("Find out more about organ donation"))
                .h2(title)
        addButtonsToExpectedPage(expected)
        return expected
    }

    private fun addButtonsToExpectedPage(expected: ExpectedPageStructure): ExpectedPageStructure {
        expected.button("NO\nI do not want to donate my organs")
                .h2("NO")
                .paragraph("I do not want to donate my organs")
                .button("YES\nI want to donate all or some of my organs")
                .h2("YES")
                .paragraph("I want to donate all or some of my organs")
        return expected
    }

    private fun getCorrectTitleText(): String {
        return if (OrganDonationSerenityHelpers.IS_AMEND_JOURNEY.isTrueOrFalse())
            amendTitle else registerTitle
    }
}
