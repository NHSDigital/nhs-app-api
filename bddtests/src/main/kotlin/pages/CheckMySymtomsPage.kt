package pages

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageObject
import pages.HybridPageElement

open class CheckMySymtomsPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val conditionsHeader = HybridPageElement(
            browserLocator = "//h2[contains(text(),'A-Z of conditions and treatments')]",
            androidLocator = null,
            page = this
    )

    val nhs111Header = HybridPageElement(
            browserLocator = "//h2[contains(text(),'Check if I need urgent help')]",
            androidLocator = null,
            page = this
    )

    fun isConditionsHeaderVisible(): Boolean {
        return conditionsHeader.element.isVisible
    }

    fun isNhs111HeaderVisible(): Boolean {
        return nhs111Header.element.isVisible
    }
}
