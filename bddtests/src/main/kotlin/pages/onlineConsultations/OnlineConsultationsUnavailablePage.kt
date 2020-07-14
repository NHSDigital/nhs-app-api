package pages.onlineConsultations

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class OnlineConsultationsUnavailablePage: HybridPageObject() {
    private val headingLocator = "//h1[contains(text(),'Online consultations are currently unavailable')]";

    private val gpAdviceHeading = HybridPageElement(
        webDesktopLocator = "$headingLocator" +
            "//span[@data-purpose='header-caption'][contains(text(), 'Ask your GP for advice')]",
        page = this)

    private val adminHelpHeading = HybridPageElement(
        webDesktopLocator = "$headingLocator" +
            "//span[@data-purpose='header-caption'][contains(text(), 'Additional GP services')]",
        page = this)

    private val info = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='olc-unavailable']" +
            "//p[@data-purpose='info']" +
            "[contains(text(), \"This service is normally available during your GP surgery's opening hours.\")]",
        page = this)

    private val coronavirusHeading = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='olc-unavailable']" +
            "//h2[@data-purpose='coronavirus-heading']" +
            "[contains(text(), 'If you think you might have coronavirus')]",
        page = this)

    private val coronavirusInfo = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='olc-unavailable']" +
            "//p[@data-purpose='coronavirus-info']" +
            "[contains(text(), 'Stay at home and avoid close contact with other people.')]",
        page = this)

    private val coronavirusLink = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='olc-unavailable']" +
            "//a[@data-purpose='coronavirus-link'][@href='https://111.nhs.uk/service/COVID-19/']" +
            "[contains(text(), 'Use the 111 coronavirus service to see if you need medical help')]",
        page = this)

    fun assertIsVisible(gpAdvice: Boolean) {
        if (gpAdvice) {
            gpAdviceHeading.assertIsVisible()
        } else {
            adminHelpHeading.assertIsVisible()
        }
        info.assertIsVisible()
        coronavirusHeading.assertIsVisible()
        coronavirusInfo.assertIsVisible()
        coronavirusLink.assertIsVisible()
    }
}
