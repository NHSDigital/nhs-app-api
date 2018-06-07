package features.authentication.steps

import pageobjects.AuthReturnPage
import org.junit.Assert


open class AuthReturnSteps {
    lateinit var authReturnPage: AuthReturnPage

    fun assertSpinnerVisible() {
        Assert.assertTrue(authReturnPage.spinnerVisible())
    }
}