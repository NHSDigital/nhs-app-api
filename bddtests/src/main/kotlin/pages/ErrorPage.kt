package pages

class ErrorPage: HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val parent = HybridPageElement(
            browserLocator = "//div[@class='msg error']",
            androidLocator = null,
            page = this
    )

    val heading = HybridPageElement(
            browserLocator = "//div[@class='msg error']/p[@class='header']",
            androidLocator = null,
            page = this
    )

    val subHeading = HybridPageElement(
            browserLocator = "//div[@class='msg error']/p[1]",
            androidLocator = null,
            page = this
    )

    val detailOne = HybridPageElement(
            browserLocator = "//div[@class='msg error']/p[2]",
            androidLocator = null,
            page = this
    )

    val detailTwo = HybridPageElement(
            browserLocator = "//div[@class='msg error']/p[3]",
            androidLocator = null,
            page = this
    )

    val button = HybridPageElement(
            browserLocator = "//button",
            androidLocator = null,
            page = this
    )


    fun hasHeading(text: String): Boolean {
        return heading.element.text.equals(text)
    }

    fun hasSubHeading(text: String): Boolean {
        return subHeading.element.text.equals(text)
    }

    fun hasDetailParagraphOne(text: String): Boolean {
        return detailOne.element.text.equals(text)
    }

    fun hasDetailParagraphTwo(text: String): Boolean {
        return detailTwo.element.text.equals(text)
    }

    fun hasButton(text: String): Boolean {
        return button.element.text.equals(text)
    }

    fun shouldNotBeVisible() {
        parent.shouldNotBeVisible()
    }

}