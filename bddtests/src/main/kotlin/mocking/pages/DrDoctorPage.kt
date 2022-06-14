package mocking.pages

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class DrDoctorPage: HybridPageObject() {

    private val pageTitle = HybridPageElement(
        webDesktopLocator = "//h1",
        page = this
    ).withText("DrDoctor")

    fun assertTitleVisible() {
        pageTitle.assertIsVisible()
    }
}
