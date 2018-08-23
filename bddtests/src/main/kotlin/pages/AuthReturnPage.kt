package pages

import org.junit.Assert.assertTrue

class AuthReturnPage : HybridPageObject() {
    val errorPageTitle = "Session error"
    val errorH1 = "Session error"
    val errorH2 = "There's been a problem loading this page"
    val errorParagraph1 = "Go back to the home screen and log in again."
    val errorParagraph2 = "If the problem continues and you need to book an appointment or get a prescription now, " +
            "contact your GP surgery directly. For urgent medical advice, call 111."
    val errorCtaText = "Back to home"

    fun assertSpinnerVisible() {
        assertTrue(spinner.element.isVisible)
    }
}

