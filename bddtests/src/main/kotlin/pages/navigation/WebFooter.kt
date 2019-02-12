package pages.navigation

import pages.HybridPageObject
import pages.HybridPageElement

open class WebFooter :  HybridPageObject() {

    val homeLink = HybridPageElement(
            webDesktopLocator = "//*[@data-purpose='homeLink']",
            page = this
    )
    val nhsSitesLink = HybridPageElement(
            webDesktopLocator = "//a[@data-purpose='nhsSitesLink']",
            page = this
    )
    val aboutUsLink = HybridPageElement(
            webDesktopLocator = "//a[@data-purpose='aboutUsLink']",
            page = this
    )

    val contactUsLink = HybridPageElement(
            webDesktopLocator = "//a[@data-purpose='contactUsLink']",
            page = this
    )

    val siteMapLink = HybridPageElement(
            webDesktopLocator = "//a[@data-purpose='siteMapLink']",
            page = this
    )

    val accessibilityInformationLink = HybridPageElement(
            webDesktopLocator = "//a[@data-purpose='accessibilityInformationLink']",
            page = this
    )

    val policiesLink = HybridPageElement(
            webDesktopLocator = "//a[@data-purpose='policiesLink']",
            page = this
    )

    fun homeLink() {
        homeLink.click()
    }

    fun nhsSitesLink() {
        nhsSitesLink.click()
    }

    fun aboutUsLink() {
        aboutUsLink.click()
    }

    fun contactUsLink() {
        contactUsLink.click()
    }

    fun siteMapLink() {
        siteMapLink.click()
    }

    fun accessibilityInformationLink() {
        accessibilityInformationLink.click()
    }

    fun policiesLink() {
        policiesLink.click()
    }


}
