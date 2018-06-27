package features.authentication.steps

import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.HomePage
import pages.navigation.Header

open class HomeSteps {

    lateinit var homePage: HomePage
    lateinit var header: Header

    @Step
    fun assertPageIsVisible() {
        assertHeaderVisible()
    }

    @Step
    fun assertWelcomeMessageShownFor(name: String) {
        Assert.assertTrue("Welcome message did not match $name", homePage.hasWelcomeMessageFor(name))
    }

    @Step
    fun assertHeaderVisible() {
        Assert.assertTrue(header.isVisible(homePage.headerText))
    }
}