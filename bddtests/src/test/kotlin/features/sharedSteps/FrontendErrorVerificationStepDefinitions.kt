package features.sharedSteps

import cucumber.api.java.en.Then
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject

class FrontendErrorVerificationStepDefinitions : HybridPageObject() {
    private val errorCode = HybridPageElement(
            webDesktopLocator = "//*[@id=\"errorCode\"]",
            androidLocator = null,
            page = this,
            helpfulName = "error message"
    )

    @Then("^In the error message I see the service reference number prefix with \"([^\"]*)\"$")
    fun iSeeTheServiceReferenceNumberPrefix(prefix: String) {
        Assert.assertEquals("the error code prefix does not match the expected prefix",
                prefix.toLowerCase(), errorCode.textValue.substring(0,2).toLowerCase())
    }

    @Then("^The reference error and label and prefix is shown as \"([^\"]*)\"$")
    fun iSeeTheServiceReferenceNumberPrefixAndLabel(prefix: String) {
        Assert.assertTrue(
                "the error code label and prefix do not match the expected label and prefix",
                errorCode.textValue.toLowerCase().startsWith(prefix.toLowerCase()))

    }
}
