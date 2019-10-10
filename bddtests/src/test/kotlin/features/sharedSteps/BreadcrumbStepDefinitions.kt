package features.sharedSteps

import cucumber.api.java.en.When
import pages.navigation.BreadcrumbHeader

open class BreadcrumbStepDefinitions {

    private lateinit var breadcrumbs : BreadcrumbHeader

    @When("^I click the '(.*)' breadcrumb$")
    fun iClickTheBreadcrumbLink(link: String) {
        breadcrumbs.selectBreadcrumbLink(link)
    }
}
