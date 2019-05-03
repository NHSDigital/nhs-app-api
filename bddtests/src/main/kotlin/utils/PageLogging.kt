package utils

import config.Config
import org.openqa.selenium.WebDriver

class PageLogging( val driver: WebDriver ) {
    fun logSelectorAndSource(selector: String) {
        if (Config.instance.showPageSourceForXPathQuery == "true") {
            println("Selector: $selector")
            println("Current source:\n${driver.pageSource}")
        }
    }
}