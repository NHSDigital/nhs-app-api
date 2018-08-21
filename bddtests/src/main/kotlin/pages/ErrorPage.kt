package pages

class ErrorPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {
    private val errorTextFinderFormat = "//div[@data-purpose='error']/p[@data-purpose='%s']"

    private val headerLocator = String.format(errorTextFinderFormat, "msg-header")
    private val subHeaderLocator = String.format(errorTextFinderFormat, "msg-subheader")
    private val messageTextLocator = String.format(errorTextFinderFormat, "msg-text")
    private val extraMessageTextLocator = String.format(errorTextFinderFormat, "msg-extratext")
    private val backButtonLocator = "//button[@data-purpose='retry-or-back-button']"

    val heading = findElementByLocator(headerLocator)

    val subHeading = findElementByLocator(subHeaderLocator)

    val errorText1 = findElementByLocator(messageTextLocator)

    val errorText2 = findElementByLocator(extraMessageTextLocator)

    val button = findElementByLocator(backButtonLocator)

    private fun findElementByLocator(locator: String): HybridPageElement {
        return HybridPageElement(
                browserLocator = locator,
                androidLocator = null,
                page = this
        )
    }

    fun hasButton(text: String): Boolean {
        return try {
            button.element.text == text
        } catch (e: Exception) {
            false
        }

    }

}