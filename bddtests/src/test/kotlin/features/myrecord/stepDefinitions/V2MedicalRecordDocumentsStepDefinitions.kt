package features.myrecord.stepDefinitions

import constants.Supplier
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import features.myrecord.factories.DocumentStatus
import features.myrecord.factories.DocumentsFactory
import models.ExpectedDocument
import org.junit.Assert
import pages.ErrorPage
import pages.assertElementNotPresent
import pages.assertIsVisible
import pages.myrecord.MyRecordDocumentInformationPage
import pages.myrecord.MyRecordDocumentPage
import pages.myrecord.MyRecordDocumentsPage
import utils.SerenityHelpers
import java.lang.IllegalStateException

open class V2MedicalRecordDocumentsStepDefinitions {

    private lateinit var myRecordDocumentInformationPage: MyRecordDocumentInformationPage
    private lateinit var myRecordDocumentsPage: MyRecordDocumentsPage
    private lateinit var myRecordDocumentPage: MyRecordDocumentPage
    private lateinit var errorPage: ErrorPage

    enum class SerenityVariable {
        EXPECTED_DOCUMENTS,
        AVAILABLE_DOCUMENT,
        UNAVAILABLE_DOCUMENT,
        SELECTED_DOCUMENT,
    }

    @Given("^the Patient has no access to Documents$")
    fun thePatientHasNoAccessToDocuments() {
        DocumentsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .disabled(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has no documents$")
    fun thePracticeHasNoDocuments() {
        DocumentsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithNoDocuments(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has a document with a null page count$")
    fun thePracticeHasDocumentNullPageCount() {
        DocumentsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithNullPageCount()
    }

    @Given("^the GP Practice has a document with a null size property$")
    fun thePracticeHasDocumentNullSizeProperty() {
        DocumentsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithNullSize()
    }

    @Given("^the GP Practice has multiple documents$")
    fun theGpPracticeHasMultipleDocuments() {
        DocumentsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithDocuments(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has multiple large documents$")
    fun theGpPracticeHasMultipleLargeDocuments() {
        DocumentsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithDocuments(SerenityHelpers.getPatient(), DocumentStatus.IsLarge)
    }

    @Given("^the GP Practice has documents with invalid types$")
    fun theGpPracticeHasDocumentsWithInvalidTypes() {
        DocumentsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithDocuments(SerenityHelpers.getPatient(), DocumentStatus.HasInvalidType)
    }

    @Given("^the GP Practice has multiple documents with no name or term$")
    fun theGpPracticeHasMultipleDocumentsWithNoNameOrTerm() {
        DocumentsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithDocumentsWithNoNameOrTerm(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has multiple non-viewable documents$")
    fun theGpPracticeHasMultipleNoViewableDocuments() {
        DocumentsFactory
            .getForSupplier(SerenityHelpers.getGpSupplier())
            .enabledWithDocuments(SerenityHelpers.getPatient(), DocumentStatus.HasNonViewableType)
    }

    @Given("^the GP practice has a file that is still uploading$")
    fun theGpPracticeHasAFileThatIsStillUploading() {
        DocumentsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithDocuments(SerenityHelpers.getPatient(),DocumentStatus.StillUploading)
    }

    @Given("^the GP Practice has multiple letters with no name or term$")
    fun theGpPracticeHasMultipleLettersWithNoNameOrTerm() {
        DocumentsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithLettersWithNoNameOrTerm(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has multiple documents where one has an invalid id$")
    fun theGpPracticeHasMultipleDocumentsWhereOneHasAnInvalidId(){
        theGpPracticeHasMultipleDocuments()
        DocumentsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithDocuments(SerenityHelpers.getPatient(), DocumentStatus.MockUnavailableDocument)
    }

    @Given("^the EMIS GP Practice has three document results where the first record has no date$")
    fun theEmisGpPracticeHasThreeDocumentResultsWhereTheFirstRecordHasNoDate(){
        DocumentsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithDocumentsWithUnknownDate(SerenityHelpers.getPatient(), false)
    }

    @When("^I select an available document$")
    fun iSelectAnAvailableDocument() {
        clickDocumentBySerenityVariable(SerenityVariable.AVAILABLE_DOCUMENT)
    }

    @When("^I select a document that has an invalid id$")
    fun iSelectADocumentThatHasAnInvalidId() {
        clickDocumentBySerenityVariable(SerenityVariable.UNAVAILABLE_DOCUMENT)
    }

    @When("^I click the (.*) action link on the document information page$")
    fun iClickTheActionLinkONTheDocumentInformationPage(action: String) {
        when (action) {
            "View" -> myRecordDocumentInformationPage.viewActionLink.click()
            "Download" -> myRecordDocumentInformationPage.downloadActionLink.click()
            else -> throw IllegalArgumentException("$action is not a valid document action")
        }
    }

    @Then("^I see the document information page with actions$")
    fun iSeeTheDocumentInformationPageWithActions() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val selectedDocument = SerenityHelpers.getValueOrNull<ExpectedDocument>(SerenityVariable.SELECTED_DOCUMENT)!!
        myRecordDocumentInformationPage.assertDocumentActionsVisible(selectedDocument.actions)
        when (gpSystem) {
            Supplier.EMIS -> {
                myRecordDocumentInformationPage.documentInfoContains(selectedDocument.date)
                myRecordDocumentInformationPage.headerContainsText(selectedDocument.term!!)
            }
            Supplier.TPP -> {
                myRecordDocumentInformationPage.headerContainsText(selectedDocument.date)
            }
            else -> {
                throw IllegalArgumentException("${gpSystem.supplierName} not implemented for Medical Record Documents")
            }
        }
    }

    @Then("^I see the document information page with download action only$")
    fun iSeeTheDocumentInformationPageWithDownloadActionOnly() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val selectedDocument = SerenityHelpers.getValueOrNull<ExpectedDocument>(SerenityVariable.SELECTED_DOCUMENT)!!

        myRecordDocumentInformationPage.viewActionLink.assertElementNotPresent()
        myRecordDocumentInformationPage.downloadActionLink.assertIsVisible()

        when (gpSystem) {
            Supplier.EMIS -> {
                throw IllegalStateException("This step has not been implemented for EMIS")
            }
            Supplier.TPP -> {
                myRecordDocumentInformationPage.headerContainsText(selectedDocument.date)
            }
            else -> {
                throw IllegalArgumentException("${gpSystem.supplierName} not implemented for Medical Record Documents")
            }
        }
    }

    @Then("^I see the document information page without actions")
    fun iSeeTheDocumentInformationPageWithNoActions() {
        val selectedDocument = SerenityHelpers.getValueOrNull<ExpectedDocument>(SerenityVariable.SELECTED_DOCUMENT)!!
        myRecordDocumentInformationPage.documentInfoContains("To access it, contact your GP surgery")
        myRecordDocumentInformationPage.headerContainsText(
                "The document added on " + selectedDocument.date + " is not available through the NHS App")
    }

    @Then("^The file has been downloaded")
    fun theFileHasBeenDownloaded() {
        val selectedDocument = SerenityHelpers.getValueOrNull<ExpectedDocument>(SerenityVariable.SELECTED_DOCUMENT)!!
        val supplier = SerenityHelpers.getGpSupplier()
        val fileName = if (supplier == Supplier.EMIS) {
            "${selectedDocument.term}.pdf"
        } else {
            "${selectedDocument.term} added on ${selectedDocument.date}.jpg"
        }
        Assert.assertTrue(myRecordDocumentInformationPage.hasFileDownloaded(fileName))
    }

    @Then("^I see a list of documents$")
    fun iSeeAListOfDocuments() {
        myRecordDocumentsPage
            .assertDocumentItemsVisible(
                SerenityHelpers.getValueOrNull<List<ExpectedDocument>>(SerenityVariable.EXPECTED_DOCUMENTS)!!)
    }

    @Then("^I see the appropriate error message for a document server error$")
    fun iSeeTheAppropriateErrorMessageForADocumentServerError() {
        errorPage.assertPageHeader(myRecordDocumentPage.serverErrorPageHeader)
                .assertHeaderText(myRecordDocumentPage.serverErrorHeader)
                .assertMessageText(myRecordDocumentPage.serverErrorMessage)
                .assertNoRetryButton()
    }

    @Then("^I can see my document$")
    fun iCanSeeMyDocument() {
        myRecordDocumentPage.document.assertIsVisible()
    }

    @Then("^I see the document information page with the document date as the header$")
    fun thenISeeTheDocumentInformationPageWithTheDocumentDateAsTheHeader() {
        val selectedDocument = SerenityHelpers.getValueOrNull<ExpectedDocument>(SerenityVariable.SELECTED_DOCUMENT)!!
        myRecordDocumentInformationPage.headerContainsText("Document added on ${selectedDocument.date}")
    }

    @Then("^I see the document information page with the document header$")
    fun thenISeeTheDocumentInformationPageWithTheDocumentHeader() {
        val selectedDocument = SerenityHelpers.getValueOrNull<ExpectedDocument>(SerenityVariable.SELECTED_DOCUMENT)!!
        myRecordDocumentInformationPage.headerContainsText("Document added on ${selectedDocument.date}")
    }

    @Then("^I see the document information page with the letter header$")
    fun thenISeeTheDocumentInformationPageWithTheLetterHeader() {
        val selectedDocument = SerenityHelpers.getValueOrNull<ExpectedDocument>(SerenityVariable.SELECTED_DOCUMENT)!!
        myRecordDocumentInformationPage.headerContainsText(
                "Letter added on ${selectedDocument.date}")
    }

    @Then("^I see the document information page with comments$")
    fun thenISeeTheDocumentInformationPageWithComments() {
        myRecordDocumentInformationPage.documentCommentContains("some comments")
    }

    @Then("^I see the expected list of documents displayed with unknown date for the last result$")
    fun thenISeeTheExpectedListOfDocumentsDisplayedWithUnknownDateForTheLastResult(){
        myRecordDocumentsPage
                .assertDocumentItemsVisible(
                        SerenityHelpers.getValueOrNull<List<ExpectedDocument>>(SerenityVariable.EXPECTED_DOCUMENTS)!!)

    }

    private fun clickDocumentBySerenityVariable(documentToClick: SerenityVariable) {
        val document = SerenityHelpers.getValueOrNull<ExpectedDocument>(documentToClick)!!
        SerenityHelpers.setSerenityVariableIfNotAlreadySet(SerenityVariable.SELECTED_DOCUMENT,document)
        myRecordDocumentsPage.clickDocument(document)
    }
}
