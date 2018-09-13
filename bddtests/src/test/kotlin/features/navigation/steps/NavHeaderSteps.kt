package features.navigation.steps

import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.HomePage
import pages.navigation.Header

open class NavHeaderSteps {

    lateinit var header: Header
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
