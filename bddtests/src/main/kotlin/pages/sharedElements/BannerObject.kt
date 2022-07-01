package pages.sharedElements

import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.WebElement
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.assertSingleElementPresent
import pages.withNormalisedText

class BannerObject private constructor(private val page : HybridPageObject,
                                       private val title: String,
                                       dialogId :String) {

    private val innerXPath = "div[@data-purpose='$dialogId']"
    private val containerXPath = "//div[$innerXPath]"

    private val container = HybridPageElement(
        containerXPath,
        page = page,
        helpfulName = "$title Banner Container")

    fun assertVisible(expectedText: String) {
        assertVisible(arrayListOf(expectedText))
    }

    fun assertVisible(expectedText: ArrayList<String>) {

        container.assertSingleElementPresent().assertIsVisible()

        container.actOnTheElement {

            val bannerTitle = it.findElement<WebElement>(By.xpath("./h2[1]")).text
            Assert.assertEquals("Expected banner title", title, bannerTitle)

            val bannerText = it.findElements<WebElement>(By.xpath("./$innerXPath/*"))
                    .map { element -> element.text }

            val message = "Expected banner text. " +
                    "Expected: ${expectedText.joinToString()}. " +
                    "Actual: ${bannerText.joinToString()}."
            Assert.assertEquals(message, expectedText.count(), bannerText.count())
            Assert.assertTrue(message, expectedText.containsAll(bannerText))
        }
    }

    fun assertFormErrorSummaryVisible(expectedText: ArrayList<String>) {

        container.assertSingleElementPresent().assertIsVisible()

        container.actOnTheElement {

            val bannerText = it.findElements<WebElement>(By.xpath("./$innerXPath/*"))
                .map { element -> element.text }

            val message = "Expected banner text. " +
                    "Expected: ${expectedText.joinToString()}. " +
                    "Actual: ${bannerText.joinToString()}."
            Assert.assertEquals(message, expectedText.count(), bannerText.count())
            Assert.assertTrue(message, expectedText.containsAll(bannerText))
        }
    }

    fun assertFormErrorSummaryHeading(heading: String) {
        assertMessage("/h2", heading)
    }

    fun assertFormErrorSummaryReasonItem(reason: String) : BannerObject {
        assertMessage("/div/ul/li", reason)
        return this
    }

    private fun assertMessage(locator: String, message: String) {
        val element = HybridPageElement(
                "//$innerXPath$locator",
                page = page).withNormalisedText(message)

        element.assertIsVisible()
    }

    companion object {

        fun success(page: HybridPageObject, title: String? = null): BannerObject {
            return BannerObject(page, title ?: "Success", "success")
        }

        fun error(page: HybridPageObject): BannerObject {
            return BannerObject(page, "Error", "error")
        }

        fun warning(page: HybridPageObject, title: String? = null): BannerObject {
            return BannerObject(page, title ?: "Warning", "warning")
        }
    }
}
