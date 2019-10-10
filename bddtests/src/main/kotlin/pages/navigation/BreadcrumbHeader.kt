package pages.navigation

import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import pages.HybridPageObject
import pages.HybridPageElement
import pages.assertIsVisible

private const val RETRY_COUNT = 3
open class BreadcrumbHeader : HybridPageObject() {
    private val breadcrumbContainerXPath = "//ol[@class='nhsuk-breadcrumb__list']"
    private val breadcrumbLinksXPath = "$breadcrumbContainerXPath/li/a"

    fun assertVisible() {
        HybridPageElement(
                webDesktopLocator = breadcrumbContainerXPath,
                webMobileLocator = breadcrumbContainerXPath,
                page = this
        ).assertIsVisible()
    }

    fun selectBreadcrumbLink(link: String) {
        val links = getAllBreadcrumbLinks()
        var retryCount = RETRY_COUNT
        while (retryCount > 0) {
            val singleLink = links.singleOrNull { element -> element.textValue.trim() == link }
            if (singleLink != null) {
                singleLink.click()
                retryCount = 0
            }
            if (singleLink == null && retryCount == 1) {
                Assert.fail("Expected breadcrumb with value: $link. " +
                        "Actual breadcrumbs: ${links.map { l -> l.textValue }.joinToString()}")
            }
        }
    }

    private fun getAllBreadcrumbLinks(): List<WebElementFacade> {
        return HybridPageElement(
                webDesktopLocator = breadcrumbLinksXPath,
                webMobileLocator = breadcrumbLinksXPath,
                page = this
        ).elements
    }
}
