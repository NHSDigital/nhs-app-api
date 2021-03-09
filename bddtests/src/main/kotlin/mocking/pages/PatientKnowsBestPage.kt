package mocking.pages

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class PatientKnowsBestPage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
            webDesktopLocator = "//h1",
            androidLocator = null,
            page = this
    ).withText("Patient Knows Best")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
