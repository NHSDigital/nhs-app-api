package features.navigation.steps

import net.thucydides.core.annotations.Step
import net.thucydides.core.annotations.Steps
import pages.HomePage
import pages.navigation.HeaderNative
import pages.navigation.WebHeader

open class NavHeaderSteps {

    lateinit var header: HeaderNative
    lateinit var homePage: HomePage

    @Steps
    lateinit var webHeader: WebHeader

    @Step
    fun clickMyAccount() {
        header.clickMyAccount()
    }

    @Step
    fun clickHelp() {
        header.clickHelp()
    }

    @Step
    fun clickHome() {
        header.clickHome()
    }

    @Step
    fun assertHomePageHeaderVisible() {
        webHeader.getPageTitle().withText(homePage.headerText)
    }
}
