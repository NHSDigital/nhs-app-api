package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.DocumentsFactory
import models.ExpectedDocument
import org.junit.Assert
import pages.ErrorPage
import pages.assertIsVisible
import pages.myrecord.MyRecordDocumentInformationPage
import pages.myrecord.MyRecordDocumentPage
import pages.myrecord.MyRecordDocumentsPage
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers
import pages.isCurrentlyEnabled

open class MyRecordDocumentsStepDefinitions : AbstractDemographicsStepDefinitions() {

    private lateinit var myRecordDocumentInformationPage: MyRecordDocumentInformationPage
    private lateinit var myRecordDocumentsPage: MyRecordDocumentsPage
    private lateinit var myRecordDocumentPage: MyRecordDocumentPage
    private lateinit var myRecordInfoPage: MyRecordInfoPage
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
                .enabledWithDocumentsWithNoNameOrTerm(SerenityHelpers.getPatient(), true)
    }

    @Given("^the GP Practice has documents with invalid types$")
    fun theGpPracticeHasDocumentsWithInvalidTypes() {
        DocumentsFactory
                .getForSupplier(SerenityHelpers.getGpSupplier())
                .enabledWithDocuments(SerenityHelpers.getPatient(), hasInvalidType = true)
    }

    @Given("^the GP Practice has multiple documents with no name or term$")
    fun theGpPracticeHasMultipleDocumentsWithNoNameOrTerm() {
        DocumentsFactory
            .getForSupplier(SerenityHelpers.getGpSupplier())
            .enabledWithDocumentsWithNoNameOrTerm(SerenityHelpers.getPatient())
    }

    @Given("^the GP Practice has multiple documents where one has an invalid id$")
    fun theGpPracticeHasMultipleDocumentsWhereOneHasAnInvalidId(){
        theGpPracticeHasMultipleDocuments()
        DocumentsFactory
            .getForSupplier(SerenityHelpers.getGpSupplier())
            .enabledWithDocuments(SerenityHelpers.getPatient(), mockUnavailableDocument = true)
    }

    @When("^I select to view my documents")
    fun iSelectToViewMyDocuments() {
        myRecordInfoPage.documents.toggleShrub()
        myRecordInfoPage.documents.documentsLink.click()
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
            else -> throw IllegalArgumentException("$action is not a valid document action")
        }
    }

    @Then("^I see the document information page with actions$")
    fun iSeeTheDocumentInformationPageWithActions() {
        val selectedDocument = SerenityHelpers.getValueOrNull<ExpectedDocument>(SerenityVariable.SELECTED_DOCUMENT)!!
        myRecordDocumentInformationPage.assertDocumentActionsVisible(selectedDocument.actions)
        myRecordDocumentInformationPage.documentInfoContains(selectedDocument.date)
        myRecordDocumentInformationPage.headerContainsText("Name")
    }

    @Then("^I see the document information page without actions")
    fun iSeeTheDocumentInformationPageWithNoActions() {
        val selectedDocument = SerenityHelpers.getValueOrNull<ExpectedDocument>(SerenityVariable.SELECTED_DOCUMENT)!!
        myRecordDocumentInformationPage.documentInfoContains("To access it, contact your GP surgery")
        myRecordDocumentInformationPage.headerContainsText(
                "The document added on " + selectedDocument.date + " is not available through the NHS App")
    }


    @Then("^I see a list of documents$")
    fun iSeeAListOfDocuments() {
        myRecordDocumentsPage
            .assertDocumentItemsVisible(
                SerenityHelpers.getValueOrNull<List<ExpectedDocument>>(SerenityVariable.EXPECTED_DOCUMENTS)!!)
    }

    @Then("The download action item is enabled")
    fun iClickTheDocumentDownloadLink(){
        Assert.assertTrue(myRecordDocumentInformationPage.downloadActionLink.isCurrentlyEnabled)
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

    private fun clickDocumentBySerenityVariable(documentToClick: SerenityVariable) {
        val document = SerenityHelpers.getValueOrNull<ExpectedDocument>(documentToClick)!!
        SerenityHelpers.setSerenityVariableIfNotAlreadySet(SerenityVariable.SELECTED_DOCUMENT,document)
        myRecordDocumentsPage.clickDocument(document)
    }
}