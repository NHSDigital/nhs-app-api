package pages

class ErrorBannerPageObject(pageObject: HybridPageObject)  {

    private val errorXPath = "//div[@data-purpose='error']"

    private val errorSummarySubHeading = HybridPageElement(
            browserLocator = "$errorXPath/p",
            androidLocator = null,
            page = pageObject
    )

    private val errorSummaryBody = HybridPageElement(
            browserLocator = "$errorXPath/ul/li",
            androidLocator = null,
            page = pageObject
    )

    val subHeading = errorSummarySubHeading.element.text
    var bodyElements = errorSummaryBody.elements.map { element->element.text }
}