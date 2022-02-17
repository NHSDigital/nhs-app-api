package features.citizenId.settings

import features.sharedSteps.BrowserSteps
import io.cucumber.java.en.Then
import net.thucydides.core.annotations.Steps
import pages.citizenId.settings.AccountSettingsPage

class AccountSettingsStepDefinitions {

    @Steps
    private lateinit var browser: BrowserSteps

    lateinit var accountSettings: AccountSettingsPage

    @Then("^the nhs account settings page has opened in a new tab$")
    fun theNhsAccountSettingsPageHasOpened() {
        browser.changeTab(accountSettings.url, true)
        accountSettings.assertTitleVisible()
    }
}
