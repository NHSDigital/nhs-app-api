package features.myAccount.steps

import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.MyAccountPage

open class MyAccountSteps {

    lateinit var myAccountPage: MyAccountPage

    @Step
    fun signOut() {
        myAccountPage.signOutButton.element.click()
    }

    @Step
    fun assertSignoutButtonVisible() {
        Assert.assertTrue("Sign out button not visible, expected to be visible", myAccountPage.isSignOutButtonVisible())
    }
}