package pages

class ErrorBannerPageObject(pageObject: HybridPageObject)  {

    private val errorXPath = "//div[@data-purpose='error']"

    private val errorSummarySubHeading = HybridPageElement(
            webDesktopLocator = "$errorXPath/p",
            androidLocator = null,
            page = pageObject
    )

    private val errorSummaryBody = HybridPageElement(
            webDesktopLocator = "$errorXPath/ul/li",
            androidLocator = null,
            page = pageObject
    )

    val subHeading = errorSummarySubHeading.element.text
    var bodyElements = errorSummaryBody.elements.map { element->element.text }
}