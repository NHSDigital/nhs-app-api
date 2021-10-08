package pages.navigation

import org.junit.Assert
import org.openqa.selenium.StaleElementReferenceException
import pages.HybridPageElement
import pages.HybridPageObject
import pages.text

private const val WAIT_FOR_PAGE_MS = 3000L
private const val HEADER_RETRIES = 20

open class WebHeader : HybridPageObject() {

    private val homeIcon = HybridPageElement(
        webDesktopLocator = "//*[@id='nhs_logo']",
        page = this
    )

    private val helpAndSupportLink = HybridPageElement(
        webDesktopLocator = "//a[normalize-space(text())='Help and support']",
        page = this
    )

    private val moreLink = HybridPageElement(
        webDesktopLocator = "//a[normalize-space(text())='More']",
        page = this
    )

    private val logoutLink = HybridPageElement(
        webDesktopLocator = "//a[normalize-space(text())='Log out']",
        page = this
    )

    private val advicePageLink = HybridPageElement(
        webDesktopLocator = "//a[@data-purpose='advicePageLink']",
        page = this
    )

    private val appointmentsPageLink = HybridPageElement(
        webDesktopLocator = "//a[@data-purpose='appointmentsPageLink']",
        page = this
    )

    private val prescriptionsPageLink = HybridPageElement(
        webDesktopLocator = "//a[@data-purpose='prescriptionsPageLink']",
        page = this
    )

    private val yourHealthPageLink = HybridPageElement(
        webDesktopLocator = "//a[@data-purpose='myRecordPageLink']",
        page = this
    )

    private val messagesPageLink = HybridPageElement(
        webDesktopLocator = "//a[@data-purpose='messagesPageLink']",
        page = this
    )

    fun getPageTitle(): HybridPageElement {
        val headerXPath = "//h1"
        return HybridPageElement(
            webDesktopLocator = headerXPath,
            page = this
        )
    }

    fun getHtmlElement(element: String): HybridPageElement {
        val headerXPath = "//${element}"
        return HybridPageElement(
            webDesktopLocator = headerXPath,
            page = this
        )
    }

    fun followAppointmentHeaderLink() {
        clickAppointmentsPageLink()
        waitForPageHeaderText("Appointments")
    }

    fun followAdviceHeaderLink() {
        clickAdvicePageLink()
        waitForPageHeaderText("Advice")
    }

    fun followPrescriptionsHeaderLink() {
        clickPrescriptionsPageLink()
        waitForPageHeaderText("Prescriptions")
    }

    fun followYourHealthHeaderLink() {
        clickYourHealthPageLink()
        waitForPageHeaderText("Your health")
    }

    fun followMoreHeaderLink() {
        clickMore()
        waitForPageHeaderText("More")
    }

    fun clickHomeIcon() {
        homeIcon.click()
    }

    fun clickHelpAndSupportLink() {
        helpAndSupportLink.click()
    }

    fun clickMore() {
        moreLink.click()
    }

    fun clickLogout() {
        logoutLink.click()
    }

    fun clickAdvicePageLink() {
        advicePageLink.click()
    }

    fun clickAppointmentsPageLink() {
        appointmentsPageLink.click()
    }

    fun clickPrescriptionsPageLink() {
        prescriptionsPageLink.click()
    }

    fun clickYourHealthPageLink() {
        yourHealthPageLink.click()
    }

    fun clickMessagesPageLink() {
        messagesPageLink.click()
    }

    fun waitForPageHeaderText(expectedHeaderText: String) {
        Thread.sleep(WAIT_FOR_PAGE_MS)
        var retryAssertionsRemaining = HEADER_RETRIES
        while (retryAssertionsRemaining > 0) {
            try {
                val title = getPageTitle()
                Assert.assertEquals("Header Expected", expectedHeaderText, title.text)
                break
            } catch (e: StaleElementReferenceException) {
                Thread.sleep(WAIT_FOR_PAGE_MS)
            } catch (e: AssertionError) {
                retryAssertionsRemaining--
                if (retryAssertionsRemaining == 0) {
                    throw(e)
                }
                Thread.sleep(WAIT_FOR_PAGE_MS)
            }
        }
    }
}
