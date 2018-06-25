package features.more.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.When
import features.more.steps.MoreSteps
import net.thucydides.core.annotations.Steps


class MoreStepDefinitions {

    @Steps
    lateinit var more: MoreSteps

    @When("^I choose to set my organ donation preferences")
    fun setOrganDonationPreferences() {
        more.setOrganDonationPreferences()
    }

    @And("^the app remains on the More Page$")
    fun appRemainsOnMorePage() {
        assert(more.isDisplayed())
    }

    @Given("^I am on the More Page$")
    fun iAmOnTheMorePage() {
        assert(more.organDonationHeaderVisible())
        assert(more.organDonationDescriptionVisible())
        assert(more.organDonationButtonVisible())
    }

}