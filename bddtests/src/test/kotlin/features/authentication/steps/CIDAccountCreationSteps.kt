package features.authentication.steps

import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.loggedOut.CIDAccountCreationPage

open class CIDAccountCreationSteps {
    lateinit var accountCreationpage: CIDAccountCreationPage

    @Step
    fun assertPageIsVisible()
    {
        Assert.assertTrue(accountCreationpage.isVisible())
    }
}
