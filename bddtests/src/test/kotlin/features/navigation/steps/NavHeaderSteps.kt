package features.navigation.steps

import net.thucydides.core.annotations.Step
import pages.HomePage
import pages.navigation.HeaderNative

open class NavHeaderSteps {

    lateinit var header: HeaderNative
    lateinit var homePage: HomePage

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
        header.assertIsVisible(homePage.headerText)
    }
}
