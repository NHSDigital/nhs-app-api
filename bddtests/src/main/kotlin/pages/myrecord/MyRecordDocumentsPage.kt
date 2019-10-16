package pages.myrecord

import models.ExpectedDocument
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

    fun assertDocumentItemsVisible(expectedDocuments: List<ExpectedDocument>) {
        for(expectedDocument in expectedDocuments){
            setBaseDocumentItemPath(expectedDocument.id)
            assertEquals("Document date or type incorrect",
                expectedDocument.dateTypeAndSize,
                getDocumentDateAndType().text)
            assertEquals("Document term incorrect",
                expectedDocument.documentTerm,
                getDocumentTerm().text)
            if (expectedDocument.name != null) {
                assertEquals("Document name incorrect",
                    expectedDocument.name,
                    getDocumentName().text)
            }
        }
    }

    fun clickDocument(document: ExpectedDocument) {
        setBaseDocumentItemPath(document.id)
        getIndividualDocumentItem().click()
    }
}