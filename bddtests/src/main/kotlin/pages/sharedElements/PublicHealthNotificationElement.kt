package pages.sharedElements

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsNotVisible
import pages.assertIsVisible
import worker.models.serviceJourneyRules.PublicHealthNotification
import worker.models.serviceJourneyRules.PublicHealthNotificationType as NotificationType
import worker.models.serviceJourneyRules.PublicHealthNotificationUrgency as NotificationUrgency
import java.lang.IllegalArgumentException

class PublicHealthNotificationElement(
        val page: HybridPageObject,
        private val publicHealthNotification: PublicHealthNotification) {

    private val selectorMap: Map<PublicHealthNotificationStyle, PublicHealthNotificationSelectors> = mapOf(
        PublicHealthNotificationStyle(NotificationType.callout, NotificationUrgency.warning) to
            PublicHealthNotificationSelectors(
                "//h3[@data-purpose='warning-callout-title']",
                "//div[@data-purpose='warning-callout-body']")
    )

    private var rootElementSelector: String = ""
    private var titleElementSelector: String = ""
    private var bodyElementSelector: String = ""

    private fun findRootElement(): HybridPageElement {
        return HybridPageElement(
            webDesktopLocator = rootElementSelector,
            page = page
        )
    }

    private fun findTitleElement(): HybridPageElement {
        return HybridPageElement(
            webDesktopLocator = "$rootElementSelector$titleElementSelector",
            page = page
        )
    }

    private fun findBodyElement(): HybridPageElement {
        return HybridPageElement(
            webDesktopLocator = "$rootElementSelector$bodyElementSelector",
            page = page
        )
    }

    fun assertIsVisible() {
        setSelectors(publicHealthNotification.id, publicHealthNotification.type, publicHealthNotification.urgency)

        assertRootElement()
        assertTitleElement(publicHealthNotification.title)
        assertBodyElement(publicHealthNotification.body)
    }

    fun assertNoneAreVisible() {
        rootElementSelector = "//div.public-health-notification"

        val rootElement = findRootElement()

        rootElement.assertIsNotVisible()
    }

    private fun setSelectors(id: String, type: NotificationType, urgency: NotificationUrgency) {
        val selectors = selectorMap[PublicHealthNotificationStyle(type, urgency)]
                ?: throw IllegalArgumentException("Unknown type/urgency combination. Type: $type, Urgency: $urgency")

        rootElementSelector = "//div[@id='public-health-notification-$id']"
        titleElementSelector = selectors.title
        bodyElementSelector = selectors.body
    }

    private fun assertRootElement() {
        findRootElement().assertIsVisible()
    }

    private fun assertTitleElement(title: String) {
        val titleElement = findTitleElement()

        titleElement.assertIsVisible()
        Assert.assertEquals(title, titleElement.textValue)
    }

    private fun assertBodyElement(body: String) {
        val bodyElement = findBodyElement()

        bodyElement.assertIsVisible()
        Assert.assertTrue(bodyElement.elements.first().getAttribute("innerHTML").contains(body))
    }

    private data class PublicHealthNotificationStyle(
            var type: NotificationType,
            var urgency: NotificationUrgency
    )

    private data class PublicHealthNotificationSelectors(
            var title: String,
            var body: String
    )
}
