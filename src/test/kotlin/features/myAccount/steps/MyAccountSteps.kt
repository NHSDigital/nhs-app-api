package features.myAccount.steps

import net.thucydides.core.annotations.Step
import pages.MyAccountPage

open class MyAccountSteps {

    lateinit var myAccountPage: MyAccountPage

    @Step
    fun signOut() {
        myAccountPage.signOutButton.click()
    }
}