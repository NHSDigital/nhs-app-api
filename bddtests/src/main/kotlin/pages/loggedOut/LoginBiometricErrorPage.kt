package pages.loggedOut

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/biometric-login-error")
class LoginBiometricErrorPage : HybridPageObject() {

    private val titleLocator = "//h1[normalize-space(text())='We could not log you in']"
    private val firstParagraphLocator = "//p[@data-sid='biometricLoginErrorParagraphOne']"
    private val secondParagraphLocator = "//p[@data-sid='biometricLoginErrorParagraphTwo']"

    private val expectedParagraphOne = "Go back to the homepage and try logging in again."
    private val expectedParagraphTwo = "If you keep seeing this message, go back to the homepage " +
            "and log in using your email, password and security code."

    fun assertDisplayed() {
        val title by lazy {
            HybridPageElement(
                    titleLocator,
                    titleLocator,
                    page = this,
                    helpfulName = "header")
        }

        val paragraphOne by lazy {
            HybridPageElement(
                    firstParagraphLocator,
                    firstParagraphLocator,
                    page = this,
                    helpfulName = "firstParagraph")
        }

        val paragraphTwo by lazy {
            HybridPageElement(
                    secondParagraphLocator,
                    secondParagraphLocator,
                    page = this,
                    helpfulName = "secondParagraph")
        }

        title.assertIsVisible()
        Assert.assertEquals(
                expectedParagraphOne,
                paragraphOne.textValue)
        Assert.assertEquals(
                expectedParagraphTwo,
                paragraphTwo.textValue)
    }
}
