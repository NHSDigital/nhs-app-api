package pages.gpMedicalRecord

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject

class ProceduresPage: HybridPageObject() {

    private val proceduresHeadings = HybridPageElement(
            webDesktopLocator = "//div[contains(@class, 'inps_class_result')]/table/thead/tr/th",
            page = this
    )

    private val proceduresItems = HybridPageElement(
            webDesktopLocator = "//div[contains(@class, 'inps_class_result')]/table/tbody/tr/td",
            page = this
    )

    private val expectedHeadings = arrayOf("Onset", "Procedure / Detail", "Onset", "Procedure / Detail")

    fun assertProceduresHeadings() {
        Assert.assertEquals(expectedHeadings.size, proceduresHeadings.elements.size)
        return proceduresHeadings.elements.forEachIndexed{ index, actualHeading ->
            Assert.assertEquals(expectedHeadings[index], actualHeading.text)
        }
    }

    fun assertProceduresItems(expectedElements: Array<String>) {
        val items = proceduresItems.elements.map {item -> item.text}
        Assert.assertEquals(items.size, proceduresItems.elements.size)
        Assert.assertEquals(expectedElements.size, proceduresItems.elements.size)
        return proceduresItems.elements.forEachIndexed{ index, actualElement ->
            Assert.assertEquals(expectedElements[index], actualElement.text)
        }
    }
}
