package pages.organDonation

import mocking.data.organDonation.OrganDonationSerenityHelpers
import mocking.data.organDonation.isTrueOrFalse
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageElement
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
class OrganDonationChoicePage : OrganDonationBasePage() {

    override val titleText: String = "Register your organ donation decision"
    private val alreadyRegisteredLinkText = "Think you have registered already?"
    private val findOutMoreLinkText = "Find out more about organ donation"
    private val expectedLinksInAmendJourney = arrayOf(findOutMoreLinkText)
    private val expectedLinksInNewJourney = arrayOf(alreadyRegisteredLinkText, findOutMoreLinkText)

    val noButton = button("NO")

    val yesButton = button("YES")

    val findOutMoreLink = getLink(findOutMoreLinkText)
    val alreadyRegisteredLink = getLink(alreadyRegisteredLinkText)

    private fun button(option: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "//button[descendant::*[contains(text(),\"$option\")]]",
                webMobileLocator = "//button[descendant::*[contains(text(),\"$option\")]]",
                androidLocator = null,
                page = this,
                helpfulName = "$option option"
        )
    }

    override fun assertDisplayed() {
        title.assertIsVisible()
        noButton.assertIsVisible()
        yesButton.assertIsVisible()

        val expectedLinks = if (OrganDonationSerenityHelpers.IS_AMEND_JOURNEY.isTrueOrFalse())
            expectedLinksInAmendJourney else expectedLinksInNewJourney

        Assert.assertArrayEquals("expectedLinks", expectedLinks, getAllLinks())
    }

    private fun getAllLinks(): Array<String> {
        val allLinks = HybridPageElement(
                "//a",
                page = this)
        return allLinks.elements.map { link -> link.text }.toTypedArray()
    }
}
