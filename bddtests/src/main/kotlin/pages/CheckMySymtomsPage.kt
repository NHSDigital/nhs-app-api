package pages

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageObject
import pages.HybridPageElement

open class CheckMySymtomsPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {
    val checkSymptomsTitleLoggedOut = HybridPageElement(
            browserLocator = "//*[@id=\"mainDiv\"]/main/content/header/h1",
            androidLocator = null,
            page = this
    )

    val checkSymptomsTitleLoggedIn = HybridPageElement(
            browserLocator = "//*[@id=\"app\"]/header/h1",
            androidLocator = null,
            page = this
    )
    fun getCheckSymptomsTitleLoggedOut(): Boolean {
        return checkSymptomsTitleLoggedOut.element.isVisible
    }

    fun getCheckSymptomsTitleLoggedIn(): Boolean {
        return checkSymptomsTitleLoggedIn.element.isVisible
    }
}