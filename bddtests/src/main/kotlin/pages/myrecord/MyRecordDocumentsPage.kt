package pages.myrecord

import org.junit.Assert.assertEquals
import pages.HybridPageObject
import pages.HybridPageElement
import pages.text

class MyRecordDocumentsPage : HybridPageObject() {

    private var baseDocumentItemPath: String = ""

    private fun setBaseDocumentItemPath(documentId: String) {
        baseDocumentItemPath = "//*[@id='document-$documentId']"
    }

    private fun getIndividualDocumentItem(): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = baseDocumentItemPath,
                androidLocator = null,
                page = this)
    }

    private fun getDocumentDateAndType(): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "$baseDocumentItemPath/p[@class='document__date-and-type']",
                androidLocator = null,
                page = this)
    }

    private fun getDocumentName(): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "$baseDocumentItemPath/p[@class='document__name']",
                androidLocator = null,
                page = this)
    }

    private fun getDocumentTerm(): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "$baseDocumentItemPath//p[@class='document__term']",
                androidLocator = null,
                page = this)
    }

    fun assertDocumentItemsVisible(numDocuments: Int) {
        for(documentNumber in 1..numDocuments){
            setBaseDocumentItemPath("document-$documentNumber")

            assertEquals("Document date or type incorrect",
                    "18 February 2018 (PDF, 1MB)",
                    getDocumentDateAndType().text)
            assertEquals("Document term incorrect", "History $documentNumber", getDocumentTerm().text)
            assertEquals("Document name incorrect", "Name $documentNumber", getDocumentName().text)
        }
    }

    fun clickAvailableDocument() {
        clickDocumentById("document-1")
    }

    fun clickDocumentById(documentId: String) {
        setBaseDocumentItemPath(documentId)
        getIndividualDocumentItem().click()
    }
}