package mocking.pages

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class WellnessAndPreventionPage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1",
        page = this
    ).withText("Wellness and Prevention")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
