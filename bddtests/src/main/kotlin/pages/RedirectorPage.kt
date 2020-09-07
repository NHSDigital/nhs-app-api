package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://web.local.bitraft.io:3000/redirector/")
open class RedirectorPage : HybridPageObject() {

    fun title(title: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "//p" +
                "[@data-purpose='warning-pre-header']" +
                "[contains(text(),\"$title\")]",
                androidLocator = null,
                page = this,
                helpfulName = title
        )
    }

    var interruptionCard :InterruptionCard = InterruptionCard(
            "//*[@data-purpose='silver-integration-warning']",
            this)
}
