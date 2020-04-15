package pages.organDonation

import mocking.data.organDonation.OrganDonationSerenityHelpers
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageElement
import pages.assertIsVisible
import pages.avoidChromeWebDriverServiceCrash
import utils.isTrueOrFalse

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
class OrganDonationChoicePage : OrganDonationBasePage() {

    override val titleText: String by lazy {getCorrectTitleText()}

    private val alreadyRegisteredLinkText = "Think you have registered already?"
    private val findOutMoreLinkText = "Find out more about organ donation"
    private val expectedLinksInAmendJourney = arrayOf(findOutMoreLinkText)
    private val expectedLinksInNewJourney = arrayOf(alreadyRegisteredLinkText, findOutMoreLinkText)

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

    fun getCorrectTitleText():String{
        return  if (OrganDonationSerenityHelpers.IS_AMEND_JOURNEY.isTrueOrFalse())
            "Change your organ donation decision" else "Register your organ donation decision"
    }

    override fun assertDisplayed() {
        //Please do not delete until NHSO-8407 and NHSO-8408 are completed
        avoidChromeWebDriverServiceCrash()
        title.assertIsVisible()
        noButton.assertIsVisible()
        yesButton.assertIsVisible()

        val expectedLinks = if (OrganDonationSerenityHelpers.IS_AMEND_JOURNEY.isTrueOrFalse())
            expectedLinksInAmendJourney else expectedLinksInNewJourney

        Assert.assertTrue(getAllLinks().contains(expectedLinks[0]))
        if(!OrganDonationSerenityHelpers.IS_AMEND_JOURNEY.isTrueOrFalse()) {
            Assert.assertTrue(getAllLinks().contains(expectedLinks[1]))
        }
    }

    private fun getAllLinks(): Array<String> {
        val allLinks = HybridPageElement(
                "//a",
                page = this).elements
        val allVisibleLinks = allLinks.filter { it.isCurrentlyVisible == true }
        return allVisibleLinks.map { link -> link.text }.toTypedArray()
    }
}
