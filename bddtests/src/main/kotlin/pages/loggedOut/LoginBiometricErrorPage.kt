package pages.loggedOut

import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/biometric-login-error")
class LoginBiometricErrorPage : HybridPageObject() {

    private val titleLocator = "//h1[normalize-space(text())='Cannot log in']"
    private val firstParagraphLocator = "//p[@data-sid='biometricLoginErrorParagraphOne']"

    private val expectedParagraphOne = "You need to log in using your email, password and security code."

    fun assertDisplayed() {
        val title by lazy {
            HybridPageElement(
                titleLocator,
                page = this,
                helpfulName = "header")
        }

        val paragraphOne by lazy {
            HybridPageElement(
                firstParagraphLocator,
                page = this,
                helpfulName = "firstParagraph")
        }

        title.assertIsVisible()
        Assert.assertEquals(
                expectedParagraphOne,
                paragraphOne.textValue)
    }
}
