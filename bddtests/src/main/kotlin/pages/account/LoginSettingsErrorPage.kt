package pages.account

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.isVisible

@DefaultUrl("http://web.local.bitraft.io:3000/account/login-settings/error")
class LoginSettingsErrorPage : HybridPageObject() {

    private val cannotFindTitleLocator = "//h1[normalize-space(text())='We cannot find your %s']"
    private val cannotChangeTitleLocator = "//h1[normalize-space(text())='We could not change your " +
            "%s" +
            " settings']"
    private val cannotFindTextLocator = "//p[@data-sid='cannotFindBiometrics']"
    private val cannotChangeParagraphLocator = "//p[@data-sid='%s']"

    private val expectedCannotFindFirstParagraph = "Go back and try again."
    private val expectedCannotChangeSecondParagraph = "If you keep seeing this message, return to your settings later."

    fun assertCannotFindContentDisplayed(biometricType: String) {
        val text by lazy {
            HybridPageElement(
                    cannotFindTextLocator,
                    cannotFindTextLocator,
                    page = this,
                    helpfulName = "information")
        }
        when (biometricType) {
            "Touch ID" -> {
                assertDisplayWithInfoText(text,
                        String.format(cannotFindTitleLocator, "Touch ID"),
                        "Check that you have added a fingerprint in your device's Touch ID settings.")
            }
            "Face ID" -> {
                assertDisplayWithInfoText(text,
                        String.format(cannotFindTitleLocator, "Face ID"),
                        "Check that you have added a face scan in your device's Face ID settings.")
            }
            "Fingerprint" -> {
                assertDisplayWithInfoText(text,
                        String.format(cannotFindTitleLocator, "fingerprint"),
                        "Check that you have added a fingerprint in your device's security settings.")
            }
        }
    }

    fun assertCannotChangeContentDisplayed(biometricType: String) {
        when (biometricType) {
            "Touch ID" -> assertDisplayWithNoInfoText(String.format(cannotChangeTitleLocator, "Touch ID"))
            "Face ID" -> assertDisplayWithNoInfoText(String.format(cannotChangeTitleLocator, "Face ID"))
            "Fingerprint" -> assertDisplayWithNoInfoText(String.format(cannotChangeTitleLocator, "fingerprint"))
        }

        val firstParagraphLocator =
                String.format(cannotChangeParagraphLocator,
                        "cannotChangeBiometricsParagraphOne")

        val secondParagraphLocator =
                String.format(cannotChangeParagraphLocator,
                        "cannotChangeBiometricsParagraphTwo")

        val firstParagraph by lazy {
            HybridPageElement(
                    firstParagraphLocator,
                    firstParagraphLocator,
                    page = this,
                    helpfulName = "informationParaOne")
        }

        val secondParagraph by lazy {
            HybridPageElement(
                    secondParagraphLocator,
                    secondParagraphLocator,
                    page = this,
                    helpfulName = "informationParaTwo")
        }

        Assert.assertTrue(firstParagraph.isVisible)
        Assert.assertEquals(expectedCannotFindFirstParagraph, firstParagraph.textValue)
        Assert.assertTrue(secondParagraph.isVisible)
        Assert.assertEquals(
                expectedCannotChangeSecondParagraph,
                secondParagraph.textValue)
    }

    private fun assertDisplayWithInfoText(text: HybridPageElement, titleLocator: String, expectedText: String) {
        val title = getTitle(titleLocator)

        Assert.assertTrue(title.isVisible)
        Assert.assertTrue(text.isVisible)
        Assert.assertEquals(
                expectedText,
                text.textValue)
    }

    private fun assertDisplayWithNoInfoText(titleLocator: String) {
        val title = getTitle(titleLocator)
        Assert.assertTrue(title.isVisible)
    }

    private fun getTitle(titleLocator: String): HybridPageElement {
        val title by lazy {
            HybridPageElement(
                    titleLocator,
                    titleLocator,
                    page = this,
                    helpfulName = "header")
        }
        return title
    }
}

