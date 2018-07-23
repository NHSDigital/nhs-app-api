package features.authentication.steps

import net.thucydides.core.annotations.Step
import pageobjects.AuthReturnPage
import org.junit.Assert
import pages.prescription.PrescriptionsPage


open class AuthReturnSteps {

    lateinit var authReturnPage: AuthReturnPage

    fun assertSpinnerVisible() {
        Assert.assertTrue(authReturnPage.spinner.element.isVisible)
    }

    @Step
    fun assertCorrectErrorMessageShown(pageTitle: String,
                                       pageHeaderText: String,
                                       headerText: String,
                                       subHeaderText: String,
                                       messageText: String,
                                       retryButtonText: String){
        Assert.assertTrue("Expected error message: { " +
                "page title: $pageTitle, " +
                "page header text: $pageHeaderText, " +
                "header text: $headerText, " +
                "sub-header text: $subHeaderText, " +
                "message text: $messageText, " +
                "retry button text: $retryButtonText } ",
                authReturnPage.isErrorMessageContentCorrect(pageTitle, pageHeaderText, headerText, subHeaderText, messageText, retryButtonText))
    }
}