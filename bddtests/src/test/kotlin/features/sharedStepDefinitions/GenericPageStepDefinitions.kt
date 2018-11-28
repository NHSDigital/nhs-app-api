package features.sharedStepDefinitions

import cucumber.api.java.en.When
import pages.HybridPageObject

class GenericPageStepDefinitions {

    lateinit var genericPage: HybridPageObject

     @When("^I click the '(.*)' button$")
     fun iClickTheFindAvailableAppointmentsButton(buttonText: String) {
         genericPage.clickOnButtonContainingText(buttonText)
     }
}
