package pages.gpMedicalRecord

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject

class DiagnosisPage: HybridPageObject() {

    private val diagnosisHeadings = HybridPageElement(
            webDesktopLocator = "//div[contains(@class, 'inps_class_result')]/table/thead/tr/th",
            page = this
    )

    private val diagnosisItems = HybridPageElement(
            webDesktopLocator = "//div[contains(@class, 'inps_class_result')]/table/tbody/tr/td",
            page = this
    )

    private val expectedHeadings = arrayOf("Date", "Problem / Detail", "Date", "Problem / Detail")

    fun assertDiagnosisHeadings() {
        Assert.assertEquals(expectedHeadings.size, diagnosisHeadings.elements.size)
        return diagnosisHeadings.elements.forEachIndexed{ index, actualHeading ->
            Assert.assertEquals(expectedHeadings[index], actualHeading.text)
        }
    }

    fun assertDiagnosisItems(expectedElements: Array<String>) {
        Assert.assertEquals(expectedElements.size, diagnosisItems.elements.size)
        return diagnosisItems.elements.forEachIndexed{ index, actualElement ->
            Assert.assertEquals(expectedElements[index], actualElement.text)
        }
    }
}
