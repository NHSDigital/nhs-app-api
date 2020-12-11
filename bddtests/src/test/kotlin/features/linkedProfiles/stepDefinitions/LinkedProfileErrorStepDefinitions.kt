package features.linkedProfiles.stepDefinitions

import io.cucumber.java.en.Then
import pages.ErrorDialogPage
import pages.GpSessionError

class LinkedProfileErrorStepDefinitions {

    private lateinit var errorDialogPage: ErrorDialogPage
    private lateinit var linkedAccountsGpSessionError: GpSessionError

    @Then("^I see appropriate linked profiles try again error message when there is no GP session$")
    fun iSeeAppropriateTryAgainErrorMessageWhenThereIsNoGpSession() {
        errorDialogPage.assertParagraphText("You are not currently able to access linked profiles.")
                .assertParagraphText("This may be a temporary problem.")
                .assertPageHeader("Sorry, there is a problem getting your linked profiles information")
                .assertPageTitle("Sorry, there is a problem getting your linked profiles information")
    }

    @Then("^I see what I can do next with a linked accounts error message and reference code '(.*)'")
    fun iSeeWhatICanDoNextWithALinkedAccountsErrorMessage(prefix: String) {
        linkedAccountsGpSessionError.assertPageHeader("Sorry, linked profiles are unavailable")
                .assertReferenceCode(prefix)
                .assertParagraphText("If you need to access health services on behalf of someone else, " +
                        "contact their GP surgery directly or try again later.")
                .assertLink("Find out more about linked profiles")
                .assertParagraphText("For urgent medical advice, go to")
                .assertParagraphText("If you have accessed linked profiles before, select ")
    }
}
