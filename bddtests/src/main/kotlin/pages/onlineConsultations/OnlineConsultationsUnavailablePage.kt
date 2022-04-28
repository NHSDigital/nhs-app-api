package pages.onlineConsultations

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

class OnlineConsultationsUnavailablePage: HybridPageObject() {
    private val headingLocator = "//h1[contains(text(),'Online consultations are currently unavailable')]"

    private val gpAdviceHeading = HybridPageElement(
        webDesktopLocator = headingLocator +
            "//span[@data-purpose='header-caption'][contains(text(), 'Ask your GP for advice')]",
        page = this)

    private val adminHelpHeading = HybridPageElement(
        webDesktopLocator = headingLocator +
            "//span[@data-purpose='header-caption'][contains(text(), 'Additional GP services')]",
        page = this)

    private val info = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='olc-unavailable']" +
            "//p[@data-purpose='info']" +
            "[contains(text(), \"This service is normally available during your GP surgery's opening hours.\")]",
        page = this)

    private val urgentMedicalAdvice = HybridPageElement(
            webDesktopLocator = "//div[@data-purpose='olc-unavailable']" +
                    "//p[@aria-label='For urgent medical advice, go to 111.nhs.uk or call one one one.']" +
                    "[contains(text(), 'For urgent medical advice, go to')]",
            page = this)

    private val coronavirusLink = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='olc-unavailable']" +
            "//a[@data-purpose='coronavirus-link'][@href='http://stubs.local.bitraft.io:8080/external/nhsuk/covid']" +
            "[contains(text(), 'Find out what to do if you think you might have coronavirus')]",
        page = this)

    fun assertIsVisible(gpAdvice: Boolean) {
        if (gpAdvice) {
            gpAdviceHeading.assertIsVisible()
        } else {
            adminHelpHeading.assertIsVisible()
        }
        info.assertIsVisible()
        urgentMedicalAdvice.assertIsVisible()
        coronavirusLink.assertIsVisible()
    }
}
