package pages.myrecord

import net.serenitybdd.core.pages.WebElementFacade
import org.openqa.selenium.By
import pages.HybridPageElement
import pages.HybridPageObject

class MyRecordWrapper(header: String, pageObject: HybridPageObject) {

    private val headerPath = "//a[contains(text(),'$header')]"
    private val bodyPath = "$headerPath/following-sibling::div[1]"

    val header =
            HybridPageElement(
                    webDesktopLocator = headerPath,
                    androidLocator = null,
                    page = pageObject,
                    helpfulName = "Section Header '$header'")

    private val paragraphElements =
            HybridPageElement(
                    webDesktopLocator = "$bodyPath//p",
                    androidLocator = null,
                    page = pageObject)

    val firstParagraph by lazy { paragraphElements.element }

    val visionLink = HybridPageElement(
            webDesktopLocator = "//a[contains(text(),'View your Test Results')]",
            androidLocator = null,
            page = pageObject)

    private val recordItemElements =
            HybridPageElement(
                    webDesktopLocator = "$bodyPath//div[@data-purpose='record-item']",
                    androidLocator = null,
                    page = pageObject)

    val firstElement by lazy { recordItemElements }

    fun toggleShrub() {
        header.scrollToElement()
        header.elements.elementAt(0).click()

        Thread.sleep(SHRUB_ANIMATION_DURATION_MILLIS)
    }

    fun allRecordItems(): List<RecordItem> {
        return recordItemElements.elements.map { element -> RecordItem(element) }
    }

    fun allRecordItemLabels(): List<String> {
        return allRecordItems().map { recordItem -> recordItem.label }
    }

    fun allRecordItemBodies(): List<String> {
        return allRecordItems().flatMap { recordItem -> recordItem.bodyElements }
    }

    fun clickFirst() {
        firstElement.click()
    }
}

class RecordItem(recordItem: WebElementFacade) {

    val element = recordItem

    val label = recordItem.find<WebElementFacade>(By.tagName("span")).text

    val bodyElements = recordItem.thenFindAll(By.tagName("p")).map { element ->
        element.textValue
    }
}
