package pages

import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl


@DefaultUrl("http://localhost:3000/login")
class LoginPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    fun checkMySymptoms() {
        findByXpath("//*[@id='btn_home_symptoms']")
                .waitUntilClickable<WebElementFacade>()
                .click()
    }

    fun signIn() {
        findByXpath("//*[@data-id='login-button']").click()

        if (onMobile()) {
            switchToPage(CitizenIDPage::class.java).login("realmadmin@gmail.com", "Welcome123!")
        } else {
            // complete login until CID integration developed
            findByXpath("//*[@id='complete_login']").click()
        }
    }
}
