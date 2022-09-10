package mocking.pages

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class HealthcareCommsPage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1",
        page = this
    ).withText("HealthcareComms")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
