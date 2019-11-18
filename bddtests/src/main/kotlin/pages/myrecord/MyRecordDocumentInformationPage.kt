package pages.myrecord

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import java.lang.IllegalArgumentException

class MyRecordDocumentInformationPage : HybridPageObject() {
    private val listMenuPath = "//ul[@data-sid='action-list-menu']//a"
    private val documentInfoPath = "//div[@id='documentInfo']/p"

    private fun link(id: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "$listMenuPath[@id='$id']",
                androidLocator = null,
                page = this,
                helpfulName = "$id Link")
    }

    private fun documentInfo(date: String): HybridPageElement {
        val infoText = "Document added on $date"
        return HybridPageElement(
                webDesktopLocator = "$documentInfoPath${String.format(containsTextXpathSubstring, infoText)}",
                androidLocator = null,
                page = this,
                helpfulName = "Document Info Text")
    }

    private fun header(headerText: String): HybridPageElement {
        return HybridPageElement(
            webDesktopLocator = "//h1${String.format(containsTextXpathSubstring, headerText)}",
            androidLocator = null,
            page = this,
            helpfulName = "Header")
    }

    val viewActionLink = link("btn_viewDocument")
    val downloadActionLink = link("btn_downloadDocument")

    fun assertDocumentActionsVisible(actions: List<String>) {
        for(action in actions) {
            when (action) {
                "View" -> viewActionLink.assertIsVisible()
                "Download" -> downloadActionLink.assertIsVisible()
                else -> throw IllegalArgumentException("$action is not a supported document action")
            }
        }
    }

    fun headerContainsText(headerText: String) {
        header(headerText).waitForElement()
    }

    fun documentInfoContainsDate(date: String) {
        documentInfo(date).waitForElement()
    }
}