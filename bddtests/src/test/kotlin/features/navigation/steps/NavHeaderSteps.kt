package features.navigation.steps

import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import pages.HomePage
import pages.navigation.WebHeader

open class NavHeaderSteps {

    lateinit var homePage: HomePage

    @Steps
    lateinit var webHeader: WebHeader

    @Step
    fun clickMore() {
        webHeader.clickMore()
    }

    @Step
    fun clickHelpAndSupport() {
        webHeader.clickHelpAndSupportLink()
    }

    @Step
    fun clickHome() {
        webHeader.clickHomeIcon()
    }

    @Step
    fun assertHomePageHeaderVisible() {
        webHeader.getPageTitle().withText(homePage.headerText)
    }
}
