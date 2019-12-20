package pages.myrecord

import pages.HybridPageObject
import pages.HybridPageElement

class MyRecordDocumentPage : HybridPageObject() {

    val document =
            HybridPageElement(
                    webDesktopLocator = "//div[@id='document']" +
                                        "//span[text()='This is a test docx document.']",
                    androidLocator = null,
                    page = this)

    val serverErrorPageHeader = "Server error"
    val serverErrorHeader = "We're experiencing technical difficulties"
    val serverErrorMessage = "Try again later. If the problem continues and you need to book an appointment or get a" +
            " prescription now, contact your GP surgery directly. For urgent medical advice, call 111."
}