package pages.sharedElements

import org.junit.Assert
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject

class BannerObject private constructor(page : HybridPageObject,
                                       private val title: String,
                                       dialogId :String) {

    private val innerXPath = "div[@data-purpose='$dialogId']"
    private val containerXPath = "//div[$innerXPath]"

    private val container = HybridPageElement(
            containerXPath,
            webMobileLocator = containerXPath,
            page = page,
            helpfulName = "$title Banner Container")

    fun assertVisible(expectedText: String) {
        assertVisible(arrayListOf(expectedText))
    }

    fun assertVisible(expectedText: ArrayList<String>) {

        container.assertSingleElementPresent().assertIsVisible()

        val bannerTitle = container.element.findElement(By.xpath("./div[1]")).text
        Assert.assertEquals("Expected banner title", title, bannerTitle)

        val bannerText = container.element.findElements(By.xpath("./$innerXPath/*"))
                .map { element -> element.text }

        val message = "Expected banner text. " +
                "Expected: ${expectedText.joinToString()}. " +
                "Actual: ${bannerText.joinToString()}."
        Assert.assertEquals(message, expectedText.count(), bannerText.count())
        Assert.assertTrue(message, expectedText.containsAll(bannerText))
    }

    companion object {

        fun success(page: HybridPageObject): BannerObject {
            return BannerObject(page, "Success", "success")
        }

        fun error(page: HybridPageObject): BannerObject {
            return BannerObject(page, "Error", "error")
        }

        fun warning(page: HybridPageObject, title: String? = null): BannerObject {
            return BannerObject(page, title ?: "Warning", "warning")
        }
    }
}