package pages.citizenId.settings

import config.Config
import models.Patient
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import utils.SerenityHelpers
import java.net.URL

class AccountSettingsPage : HybridPageObject() {
    private val baseHelpUrl = "https://www.nhs.uk/nhs-app/nhs-app-legal-and-cookies"

    val url: URL = URL(Config.instance.cidSettingsUrl)

    val urls = mapOf(
        "Help and support" to ("0" to "https://www.nhs.uk/nhs-app/nhs-app-help-and-support/"),
        "Accessibility statement" to ("1" to "$baseHelpUrl/nhs-app-accessibility-statement/"),
        "Open source licences" to ("2" to "$baseHelpUrl/nhs-app-open-source-licences/"),
        "Privacy policy" to ("3" to "$baseHelpUrl/nhs-app-privacy-policy/privacy-policy/"),
        "Terms of use" to ("4" to "$baseHelpUrl/nhs-app-terms-of-use/")
    )

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

    fun assertLinkExists(linkText: String): HybridPageElement? {
        val linkConfig = urls[linkText]

        if (linkConfig == null) {
            Assert.fail("Mapping missing for link with text '$linkText'")
            return null
        }

        return assertLinkExists(
            linkConfig.second,
            "//a[@id='account-menu-item${linkConfig.first}'][contains(.,'$linkText')]")
    }
}
