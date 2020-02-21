package pages.myrecord

import pages.HybridPageObject
import pages.HybridPageElement
import pages.assertIsVisible
import pages.ELEMENT_RETRY_TIME
import pages.MILLISECONDS_IN_A_SECOND
import pages.TIME_TO_WAIT_FOR_ELEMENT
import java.io.File
import java.lang.IllegalArgumentException

class MyRecordDocumentInformationPage : HybridPageObject() {
    private val listMenuPath = "//ul[@data-sid='action-list-menu']//a"
    private val documentInfoPath = "//div[@id='documentInfo']/p"
    private val documentCommentsPath = "//span[@id='documentComment0']/pre"

    private fun link(id: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "$listMenuPath[@id='$id']",
                androidLocator = null,
                page = this,
                helpfulName = "$id Link")
    }

    private fun documentInfo(infoText: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "$documentInfoPath${String.format(containsTextXpathSubstring, infoText)}",
                androidLocator = null,
                page = this,
                helpfulName = "Document Info Text")
    }

    private fun documentComment(commentText: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "$documentCommentsPath${String.format(containsTextXpathSubstring, commentText)}",
                androidLocator = null,
                page = this,
                helpfulName = "Document Comment Text")
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

    fun hasFileDownloaded(expectedFileName: String): Boolean {
        var hasFileDownloaded  = false
        val currentDir = System.getProperty("user.dir")
        val dir = File("$currentDir/tmpDownloads")
        dir.mkdirs()

        var retryCount = (TIME_TO_WAIT_FOR_ELEMENT / ELEMENT_RETRY_TIME).toInt()
        while (!hasFileDownloaded && retryCount > 0) {
            val dirContents = dir.listFiles()

            val expected = dirContents.filter { it.name == expectedFileName }

            if (expected.isNotEmpty()) {
                hasFileDownloaded = true
                break
            }
            println("File not found, possibly still downloading. Retrying...")
            retryCount--
            Thread.sleep((ELEMENT_RETRY_TIME * MILLISECONDS_IN_A_SECOND).toLong())
        }
        return hasFileDownloaded
    }


    fun headerContainsText(headerText: String) {
        header(headerText).waitForElement()
    }


    fun documentInfoContains(text: String) {
        documentInfo(text).waitForElement()
    }

    fun documentCommentContains(text: String) {
        documentComment(text).waitForElement()
    }
}