package pages

import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.annotations.findby.How
import net.serenitybdd.core.pages.WebElementFacade

open class CIDAccountCreationPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {
    @FindBy(how = How.XPATH, using = "//*[@id='register_new_account']")
    lateinit var createAccountButton: WebElementFacade

    fun isVisible() : Boolean {
        return createAccountButton.isVisible;
    }

    fun completeAccountCreation() {
        // complete login until CID integration developed
        createAccountButton.click()
    }
}