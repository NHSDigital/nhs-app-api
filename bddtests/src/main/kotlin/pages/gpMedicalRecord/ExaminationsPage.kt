package pages.gpMedicalRecord

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject

class ExaminationsPage: HybridPageObject() {

    private val examinationsHeadings = HybridPageElement(
            webDesktopLocator = "//div[contains(@class, 'inps_class_result')]/table/thead/tr/th",
            page = this
    )

    private val examinationsItems = HybridPageElement(
            webDesktopLocator = "//div[contains(@class, 'inps_class_result')]/table/tbody/tr/td",
            page = this
    )

    private val expectedHeadings = arrayOf("Date", "Examination", "Result")

    fun assertExaminationsHeadings() {
        Assert.assertEquals(expectedHeadings.size, examinationsHeadings.elements.size)
        return examinationsHeadings.elements.forEachIndexed{ index, actualHeading ->
            Assert.assertEquals(expectedHeadings[index], actualHeading.text)
        }
    }

    fun assertExaminationsItems(expectedElements: Array<String>) {
        Assert.assertEquals(expectedElements.size, examinationsItems.elements.size)
        return examinationsItems.elements.forEachIndexed{ index, actualElement ->
            Assert.assertEquals(expectedElements[index], actualElement.text)
        }
    }
}
