package features.throttling.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import pages.assertIsVisible
import pages.throttling.GPParticipationPage

open class GpParticipationPageStepDefinitions {

    lateinit var gpParticipationPage: GPParticipationPage

    @When("^I click the This is not my GP surgery button$")
    fun iClickTheThisIsNotMyGPSurgeryButton() {
        gpParticipationPage.notMySurgeryLink.click()
    }

    @Then("^I see the Practice Not Participating page$")
    fun iSeeThePracticeNotParticipatingPage() {
        gpParticipationPage.featuresUsedHeaderNotParticipatingPractice.assertIsVisible()
        gpParticipationPage.assertNotParticipatingFeaturesVisible()
    }

    @Then("^I see the Practice Participating page$")
    fun iSeeThePracticeParticipatingPage() {
        gpParticipationPage.featuresUsedHeaderParticipatingPractice.assertIsVisible()
        gpParticipationPage.assertParticipatingFeaturesVisible()
        gpParticipationPage.createAccountMessage.assertIsVisible()
        gpParticipationPage.limitingFeaturesWarning.assertIsVisible()
    }
}

