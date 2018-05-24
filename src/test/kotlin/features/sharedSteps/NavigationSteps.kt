package features.sharedSteps

import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.navigation.NavBar

open class NavigationSteps {

    lateinit var navBar: NavBar

    @Step
    fun hasSelectedTab(tab: String): Boolean {
        return navBar.isHighlighted(NavBar.NavBarType.valueOf(tab.toUpperCase()))
    }

    @Step
    fun hasVisible(tab: String): Boolean {
        return hasVisible(NavBar.NavBarType.valueOf(tab.toUpperCase()))
    }

    private fun hasVisible(tab: NavBar.NavBarType): Boolean {
        return navBar.isVisible(tab)
    }

    @Step
    fun select(tab: String) {
        navBar.select(NavBar.NavBarType.valueOf(tab.toUpperCase()))
    }

    @Step
    fun assertVisible() {
        for (tab in NavBar.NavBarType.values()) {
            Assert.assertTrue("$tab not visible", hasVisible(tab))
        }
    }
}