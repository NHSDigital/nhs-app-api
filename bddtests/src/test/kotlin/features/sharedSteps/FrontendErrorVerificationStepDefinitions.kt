package features.sharedSteps

import io.cucumber.java.en.Then
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject

class FrontendErrorVerificationStepDefinitions : HybridPageObject() {
    private val errorCode = HybridPageElement(
        webDesktopLocator = "//*[@id=\"errorCode\"]",
        page = this,
        helpfulName = "error message"
    )

    @Then("^The reference error and label and prefix is shown as \"([^\"]*)\"$")
    fun iSeeTheServiceReferenceNumberPrefixAndLabel(prefix: String) {
        Assert.assertTrue(
                "the error code label and prefix do not match the expected label and prefix",
                errorCode.textValue.toLowerCase().startsWith(prefix.toLowerCase()))

    }
}
