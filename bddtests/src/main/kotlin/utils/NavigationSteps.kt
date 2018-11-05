package features.sharedSteps

import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.navigation.HeaderNative
import pages.navigation.NavBarNative

open class NavigationSteps {

    lateinit var navBarNative: NavBarNative
    lateinit var headerNative: HeaderNative

    @Step
    fun assertSelectedTab(tab:NavBarNative.NavBarType){
        Assert.assertTrue("Expected tab '${tab.name}' to be selected, but no tabs selected" , hasAnyTabSelected())
        Assert.assertTrue("Expected tab '${tab.name}' to be selected." , hasSelectedTab(tab))
    }

    @Step
    fun hasSelectedTab(tab: NavBarNative.NavBarType): Boolean {
        return navBarNative.isHighlighted(tab)
    }

    @Step
    fun waitForSpinnerToDisappear() {
        navBarNative.waitForSpinnerToDisappear()
    }

    @Step
    fun hasAnyTabSelected() : Boolean {
        return navBarNative.hasSingleSelection()
    }

    @Step
    fun hasVisible(tab: String): Boolean {
        return hasVisible(NavBarNative.NavBarType.valueOf(tab.toUpperCase()))
    }

    private fun hasVisible(tab: NavBarNative.NavBarType): Boolean {
        return navBarNative.isVisible(tab)
    }

    @Step
    fun select(tab: NavBarNative.NavBarType) {
        navBarNative.waitForSpinnerToDisappear()
        navBarNative.select(tab)
    }

    @Step
    fun assertVisible() {
        for (tab in NavBarNative.NavBarType.values()) {
            Assert.assertTrue("$tab not visible", hasVisible(tab))
        }
    }
}
