package features.myAccount.stepDefintions

import cucumber.api.java.en.Then
import utils.SerenityHelpers
import pages.MyAccountPage
import pages.assertIsVisible

class MyAccountStepDefinitions {

    lateinit var myAccount: MyAccountPage

    @Then("^I am on the My Account page$")
    fun iAmOnTheMyAccountPage() {
        myAccount.signOutButton.assertIsVisible()
        myAccount.isAboutUsHeaderVisible()
        myAccount.assertAllLinksVisible()
    }

    @Then("^I see my personal details")
    fun iSeeMyPersonalDetails() {

        val patient = SerenityHelpers.getPatient()
        myAccount.assertPersonalDetailsVisible(patient.formattedFullName(),
                patient.formattedDateOfBirth(),
                patient.formattedNHSNumber())
    }
}
