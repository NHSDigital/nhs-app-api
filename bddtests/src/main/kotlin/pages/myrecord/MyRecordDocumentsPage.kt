package pages.myrecord

import models.ExpectedDocument
import org.junit.Assert.assertEquals
import pages.HybridPageObject
import pages.HybridPageElement
import pages.text

class MyRecordDocumentsPage : HybridPageObject() {

    private var baseDocumentItemPath: String = ""

    private fun setBaseDocumentItemPath(documentId: String) {
        baseDocumentItemPath = "//*[@id='$documentId']"
    }

    private fun getIndividualDocumentItem(): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = baseDocumentItemPath,
                androidLocator = null,
                page = this)
    }

    private fun getDocumentDateAndType(): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "$baseDocumentItemPath//span//p",
                androidLocator = null,
                page = this)
    }

    private fun getDocumentTerm(): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "$baseDocumentItemPath//span//h2",
                androidLocator = null,
                page = this)
    }

    fun assertDocumentItemsVisible(expectedDocuments: List<ExpectedDocument>) {
        for(expectedDocument in expectedDocuments){
            setBaseDocumentItemPath(expectedDocument.id)
            if (expectedDocument.typeAndSize != null) {
                assertEquals("Document type or size incorrect",
                        expectedDocument.typeAndSize,
                        getDocumentDateAndType().text)
            }
            if (expectedDocument.term != null) {
                assertEquals("Document name or date incorrect",
                        "${expectedDocument.term} added on ${expectedDocument.date}",
                        getDocumentTerm().text)
            }else {
                assertEquals("Document date is incorrect",
                        expectedDocument.date,
                        getDocumentTerm().text)
            }
        }
    }

    fun clickDocument(document: ExpectedDocument) {
        setBaseDocumentItemPath(document.id)
        getIndividualDocumentItem().click()
    }
}