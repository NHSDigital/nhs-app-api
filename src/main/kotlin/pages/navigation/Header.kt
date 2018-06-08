package pages.navigation

import net.serenitybdd.core.pages.WebElementFacade
import org.openqa.selenium.By
import pages.HybridPageObject

class Header : HybridPageObject(Companion.PageType.NATIVE) {

    fun isVisible(title: String): Boolean {
        val logoIsVisible = find<WebElementFacade>(By.id("nhs_logo")).isVisible
        val accountIsVisible = find<WebElementFacade>(By.id("accountIcon")).isVisible
        val headingIsVisible = findBy<WebElementFacade>("//header/h1[text()='$title']").isVisible

       return logoIsVisible && accountIsVisible && headingIsVisible
    }

    fun clickMyAccount() {
        findByXpath("//a[@href='/account']/*[name()='svg']").click()
    }
}