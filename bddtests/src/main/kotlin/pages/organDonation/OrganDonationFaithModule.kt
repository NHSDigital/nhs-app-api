package pages.organDonation

import org.junit.Assert
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.assertSingleElementPresent

class OrganDonationFaithModule(page: HybridPageObject) {

    private val title = "Faith / beliefs details"

    private val containerXPath = "//div[h2[text()=\"$title\"]]"

    private val container = HybridPageElement(
            containerXPath,
            page = page,
            helpfulName = "container for '$title' section")

    init {
        container.assertSingleElementPresent().assertIsVisible()
    }

    fun assertChoice(choice: String) {
        val actualText = container.element.findElement(By.xpath("./p/b")).text

        Assert.assertEquals("Faith and Beliefs",
                "I would like NHS staff to speak to my family and anyone else appropriate about " +
                        "how organ donation can go ahead in line with my faith or beliefs",
                actualText)

        val actualChoice = container.element.findElement(By.xpath("./span")).text

        Assert.assertEquals("Faith and Beliefs Choice",
                choice,
                actualChoice)
    }
}