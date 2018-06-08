package pages

import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.annotations.findby.How
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import org.openqa.selenium.By
import org.openqa.selenium.internal.FindsByXPath
import java.util.concurrent.TimeUnit


@DefaultUrl("http://localhost:3000/login")
class LoginPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    @FindBy(how = How.XPATH, using = "//header/h1[contains(text(), 'Home')]")
    lateinit var heading: WebElementFacade

    @FindBy(how = How.XPATH, using = "//*[@id='btn_home_symptoms']")
    lateinit var symptomsButton: WebElementFacade

    @FindBy(how = How.XPATH, using = "//*[@data-id='login-button']")
    lateinit var loginButton: WebElementFacade

    @FindBy(how = How.XPATH, using = "//*[@data-id='create-account-button']")
    lateinit var createAccountButton: WebElementFacade

    lateinit var accountCreationPage : CIDAccountCreationPage

    fun checkMySymptoms() {
        symptomsButton
                .waitUntilClickable<WebElementFacade>()
                .click()
    }

    fun signIn() {
        loginButton.click()

        if (onMobile()) {
            switchToPage(CitizenIDPage::class.java).login("realmadmin@gmail.com", "Welcome123!")
        } else {
            // complete login until CID integration developed
            findByXpath("//*[@id='complete_login']").click()
        }
    }

    fun createAccount() {
        clickCreateAccountButton()

        if (onMobile()) {
            switchToPage(CitizenIDPage::class.java).login("realmadmin@gmail.com", "Welcome123!")
        } else {
            // complete login until CID integration developed
            accountCreationPage.completeAccountCreation()
        }
    }

    fun isCreateAccountButtonVisible() : Boolean {

        return createAccountButton.isVisible;
    }

    fun clickCreateAccountButton()
    {
        createAccountButton.click()
    }

    override fun shouldBeDisplayed() {
        super.shouldBeDisplayed()

        Assert.assertTrue("Heading was not displayed.", headingIsDisplayed())
        Assert.assertTrue("Buttons were not displayed.", buttonsAreDisplayed())
    }

    private fun buttonsAreDisplayed(): Boolean {
        return symptomsButton.isDisplayed
                && loginButton.isDisplayed
                && createAccountButton.isDisplayed
    }

    private fun headingIsDisplayed(): Boolean {
        return heading.isDisplayed
    }
    // Checks to see the menu item is not present on the page.
    fun assertMenuIsNotVisible() {
        Assert.assertFalse(findByXpath("//nav[@class='menu']").isVisible);
    }
}
