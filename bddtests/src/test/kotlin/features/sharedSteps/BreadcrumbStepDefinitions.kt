package features.sharedSteps

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import pages.navigation.BreadcrumbHeader

open class BreadcrumbStepDefinitions {

    private lateinit var breadcrumbs : BreadcrumbHeader

    @When("^I click the '(.*)' breadcrumb$")
    fun iClickTheBreadcrumb(link: String) {
        breadcrumbs.selectBreadcrumbLink(link)
    }

    @Then("^the breadcrumb bar is not visible$")
    fun theBackBarIsNotVisible() {
        breadcrumbs.assertNotPresent()
    }
}
