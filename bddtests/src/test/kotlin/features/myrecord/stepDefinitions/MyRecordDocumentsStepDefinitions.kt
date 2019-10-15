package features.myrecord.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.myrecord.factories.DocumentsFactory
import mocking.defaults.EmisMockDefaults
import pages.ErrorPage
import pages.assertIsVisible
import pages.myrecord.MyRecordDocumentPage
import pages.myrecord.MyRecordDocumentsPage
import pages.myrecord.MyRecordInfoPage
import utils.SerenityHelpers

open class MyRecordDocumentsStepDefinitions : AbstractDemographicsStepDefinitions() {

    private lateinit var myRecordDocumentsPage: MyRecordDocumentsPage
    private lateinit var myRecordDocumentPage: MyRecordDocumentPage
    private lateinit var myRecordInfoPage: MyRecordInfoPage
    private lateinit var errorPage: ErrorPage

    enum class SerenityVariable {
        NUMBER_OF_DOCUMENTS,
        AVAILABLE_DOCUMENT_ID,
        INVALID_DOCUMENT_ID,
    }

    @Given("^the Patient has no access to Documents$")
    fun thePatientHasNoAccessToDocuments() {
        DocumentsFactory
            .getForSupplier(SerenityHelpers.getGpSupplier())
            .disabled(EmisMockDefaults.patientEmis)
    }

    @Given("^the GP Practice has no documents$")
    fun thePracticeHasNoDocuments() {
        DocumentsFactory
            .getForSupplier(SerenityHelpers.getGpSupplier())
            .enabledWithNoDocuments(EmisMockDefaults.patientEmis)
    }

    @Given("^the GP Practice has multiple documents$")
    fun theGpPracticeHasMultipleDocuments() {
        DocumentsFactory
            .getForSupplier(SerenityHelpers.getGpSupplier())
            .enabledWithDocuments(EmisMockDefaults.patientEmis)
    }

    @Given("^the GP Practice has multiple documents where one has an invalid id$")
    fun theGpPracticeHasMultipleDocumentsWhereOneHasAnInvalidId(){
        theGpPracticeHasMultipleDocuments()
        DocumentsFactory
            .getForSupplier(SerenityHelpers.getGpSupplier())
            .withOneInvalidDocument(EmisMockDefaults.patientEmis)
    }

    @When("^I select to view my documents")
    fun iSelectToViewMyDocuments() {
        myRecordInfoPage.documents.toggleShrub()
        myRecordInfoPage.documents.documentsLink.click()
    }

    @When("^I select a document that has an invalid id$")
    fun iSelectADocumentThatHasAnInvalidId() {
        myRecordDocumentsPage.clickDocumentById(
            SerenityHelpers.getValueOrNull(SerenityVariable.INVALID_DOCUMENT_ID)!!
        )
    }

    @When("^I select an available document$")
    fun iSelectAnAvailableDocument() {
        myRecordDocumentsPage.clickAvailableDocument()
    }

    @Then("^I see a list of documents$")
    fun iSeeAListOfDocuments() {
        myRecordDocumentsPage
                .assertDocumentItemsVisible(SerenityHelpers.getValueOrNull(SerenityVariable.NUMBER_OF_DOCUMENTS)!!)
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
}