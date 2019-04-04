package features.throttling.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import org.junit.Assert
import pages.assertIsVisible
import pages.throttling.SendingEmailPage

open class SendingEmailPageStepDefinitions {

    lateinit var sendingEmailPage: SendingEmailPage

    @When("^I enter (a valid|an invalid) email and submit$")
    fun iEnterAValidOrInvalidEmailAndSubmit(validOrInvalid: String){
        when (validOrInvalid) {
            "a valid" -> {
                sendingEmailPage.enterEmail(SendingEmailPage.validEmail)
            }
            "an invalid" -> {
                sendingEmailPage.enterEmail(SendingEmailPage.invalidEmail)
            }
        }
        sendingEmailPage.continueButton.click()
    }

    @When("^I choose (to|not to) sign up to brothermailer$")
    fun iChooseToOrNotToSignUpToBrotherMailer(toOrNotTo: String) {
        when (toOrNotTo) {
            "to" -> {
                sendingEmailPage.yesRadioButton.click()
            }
            "not to" -> {
                sendingEmailPage.noRadioButton.click()
            }
        }
    }

    @When("^I click the back button on Sending Email page$")
    fun iClickTheBackButtonOSendingEmailPage() {
        sendingEmailPage.backLink.click()
    }

    @Then("^I see the invalid email error$")
    fun iSeeTheInvalidEmailError() {
        sendingEmailPage.isInvalidEmailVisible()
    }

    @Then("^I see the brothermailer service is down error$")
    fun iSeeTheBrotherMailerServiceIsDown() {
        val message = sendingEmailPage.inLineError.element.text
        Assert.assertEquals("There was a problem adding you. Please try again", message)
    }

    @Then("^I see the make a choice error$")
    fun iSeeTheMakeAChoiceError() {
        sendingEmailPage.choiceError.assertIsVisible()
    }

    @Then("^I see the Sending Email Page$")
    fun iSeeTheSendingEmailPage() {
        sendingEmailPage.waitingListResultsHeader.assertIsVisible()
        sendingEmailPage.emailFeatureText.assertIsVisible()
        sendingEmailPage.emailText.assertIsVisible()
        sendingEmailPage.continueButton.assertIsVisible()
    }
}

