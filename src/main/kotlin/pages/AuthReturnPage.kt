package pageobjects

import net.serenitybdd.core.annotations.findby.FindBy
import pages.HybridPageObject
import net.serenitybdd.core.pages.WebElementFacade

class AuthReturnPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

        @FindBy(id = "loading-spinner")
        lateinit var spinner: WebElementFacade

        fun spinnerVisible(): Boolean {
                return spinner.isCurrentlyVisible
        }
}
