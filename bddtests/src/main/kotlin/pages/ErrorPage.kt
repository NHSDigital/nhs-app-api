package pages

class ErrorPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

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

    val subHeading = paragraph(1)

    val detailOne = paragraph(2)

    val detailTwo = paragraph(3)

    val button = HybridPageElement(
            browserLocator = "//button",
            androidLocator = null,
            page = this
    )

    fun paragraph(int: Int): HybridPageElement {
        return HybridPageElement(
                browserLocator = "//div[@class='msg error']/p[$int]",
                androidLocator = null,
                page = this
        )
    }


    fun hasHeading(text: String): Boolean {
        return heading.element.text == text
    }

    fun hasSubHeading(text: String): Boolean {
        return subHeading.element.text == text
    }

    fun hasDetailParagraphOne(text: String): Boolean {
        return detailOne.element.text == text
    }

    fun hasDetailParagraphTwo(text: String): Boolean {
        return detailTwo.element.text == text
    }

    fun hasButton(text: String): Boolean {
        return try {
            button.element.text == text
        } catch (e: Exception) {
            false
        }

    }

    fun shouldNotBeVisible() {
        parent.shouldNotBeVisible()
    }

}