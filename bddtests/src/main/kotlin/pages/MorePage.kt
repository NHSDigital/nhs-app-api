package pages

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert

@DefaultUrl("http://web.local.bitraft.io:3000/more")
open class MorePage : HybridPageObject() {

    private val sections = "//div[@id='mainDiv']/ul/li//a"
    private val organDonationTitle = "Set organ donation preferences"
    private val organDonationDescription = "Help save thousands of lives in the UK every year by signing up" +
            " to become a donor on the NHS Organ Donor Register."
    private val dataSharingTitle = "Find out why your data matters"
    private val dataSharingDescription =
            "Find out how the NHS uses your confidential patient " +
            "information and choose whether or not it can be used for research and planning."

    private fun listOfLinks(): HybridPageElement {
        return HybridPageElement(
                browserLocator = sections,
                androidLocator = null,
                page = this,
                helpfulName = "ListOfLinks"
        )
    }

    private fun link(linkTitle: String): HybridPageElement {
        return HybridPageElement(
                browserLocator = "$sections[h2[contains(text(),'$linkTitle')]]",
                androidLocator = null,
                iOSLocator = "//*[contains(text(),'$linkTitle')]",
                page = this,
                helpfulName = "$linkTitle Link")
    }

    private fun linkDescriptionBody(linkTitle: String): HybridPageElement {
        return HybridPageElement(
                browserLocator = "$sections[h2[contains(text(),'$linkTitle')]]//p",
                androidLocator = null,
                page = this,
                helpfulName = "$linkTitle Link")
    }

    val btnOrganDonation  by lazy{ link(organDonationTitle)}
    val btnDataSharing by lazy{ link(dataSharingTitle)}

    val expectedLinks by
    lazy {
        arrayListOf(
                Pair(organDonationTitle, organDonationDescription),
                Pair(dataSharingTitle, dataSharingDescription))
    }

    private fun assertDescription(title: String, description: String){
        val body = linkDescriptionBody(title)
        Assert.assertEquals("Description Body",
                description,
                body.assertSingleElementPresent().element.text)
    }

    fun assertLinksPresent() {
        val links = listOfLinks().elements
        Assert.assertEquals("Number of expected Links",
                expectedLinks.count(),
                links.count())
        expectedLinks.forEach { link ->
            link(link.first).assertSingleElementPresent()
            assertDescription(link.first, link.second)
        }
    }
}
