package features.throttling.stepDefinitions

import cucumber.api.java.en.Then
import pages.assertIsVisible
import pages.throttling.GPParticipationPage

open class GpParticipationPageStepDefinitions {

    lateinit var gpParticipationPage: GPParticipationPage

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

