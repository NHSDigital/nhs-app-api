package pages.citizenId.settings

import config.Config
import models.Patient
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import utils.SerenityHelpers
import java.net.URL

class AccountSettingsPage : HybridPageObject() {
    val url: URL = URL(Config.instance.cidSettingsUrl)

    private fun pageTitle(patient: Patient): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "//h2",
                androidLocator = null,
                page = this
        ).withText("Account settings for ${patient.name.firstName} ${patient.name.surname}")
    }

    fun assertTitleVisible() {
        pageTitle(SerenityHelpers.getPatient()).assertIsVisible()
    }
}
