package pages.myrecord

import net.serenitybdd.core.pages.WebElementFacade
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject

class MyRecordWrapper(header:String , pageObject: HybridPageObject) {

    private val headerPath = "//h2[contains(text(),'$header')]"
    private val bodyPath = "$headerPath/following-sibling::div[1]"

    val header =
            HybridPageElement(
                    browserLocator = headerPath,
                    androidLocator = null,
                    page = pageObject)

    val msg =
            HybridPageElement(
                    browserLocator = bodyPath,
                    androidLocator = null,
                    page = pageObject)

    private val paragraphElements =
            HybridPageElement(
                    browserLocator = "$bodyPath//p",
                    androidLocator = null,
                    page = pageObject)

    val firstParagraph by lazy {paragraphElements.element}

    private val recordItemElements =
            HybridPageElement(
                    browserLocator = "$bodyPath//div[@data-purpose='record-item']",
                    androidLocator = null,
                    page = pageObject)

    val firstElement by lazy {recordItemElements.element}

    fun toggleShrub() {
        header.element.click()
        Thread.sleep(SHRUB_ANIMATION_DURATION_MILLIS)
    }

    fun allRecordItems(): List<WebElementFacade> {
        return recordItemElements.elements
    }

    fun allRecordItemLabels(): MutableList<WebElementFacade> {
        return msg.element.thenFindAll(By.tagName("span"))
    }

    fun allRecordItemBodies(): MutableList<WebElementFacade> {
        return msg.element.thenFindAll(By.tagName("p"))
    }

    fun clickFirst() {
        firstElement.click()
    }
}