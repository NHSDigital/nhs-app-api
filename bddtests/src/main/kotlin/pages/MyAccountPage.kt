package pages

import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.annotations.findby.How
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://localhost:3000/account")
class MyAccountPage: HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    @FindBy(how = How.XPATH, using = "//button[@id='signout-button']")
    lateinit var signOutButton: WebElementFacade


}