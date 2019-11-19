package features.sharedSteps

import pages.ErrorPage
import pages.HybridPageElement

class NotFoundErrorPage: ErrorPage() {

    private val pageTitle = "Page not found"
    private val errorHeader = "Page not found"
    private val subHeader = "If you entered a web address, check it was correct."
    private val text = "You can go directly to book an appointment or order a repeat prescription, or " +
            "use the menu buttons to find the service you need. For urgent medical advice, call 111."


    private val title by lazy {
        HybridPageElement(
                webDesktopLocator ="//h1[normalize-space(text())='$pageTitle']",
                page = this,
                helpfulName = "header")
    }

    fun assertNotFoundErrorPage() {
        title.waitForElement()
        assertHeaderText(errorHeader)
        assertSubHeaderText(subHeader)
        assertMessageText(text)
    }

}