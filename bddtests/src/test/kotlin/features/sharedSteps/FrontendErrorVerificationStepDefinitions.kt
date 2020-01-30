package features.sharedSteps

import cucumber.api.java.en.Then
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject

class FrontendErrorVerificationStepDefinitions : HybridPageObject() {

    private val errorLinkText = "?errorcode="
    private val errorCode = HybridPageElement(
            webDesktopLocator = "//span[@id=\"errorCode\"]",
            androidLocator = null,
            page = this,
            helpfulName = "error message"
    )

    @Then("^In the error message I see the service reference number prefix with \"([^\"]*)\"$")
    fun iSeeTheServiceReferenceNumberPrefix(prefix: String) {
        Assert.assertEquals("the error code prefix does not match the expected prefix",
                prefix.toLowerCase(), errorCode.textValue.substring(0,2).toLowerCase())
    }

    @Then("^Contact us link is appended with the error code as a query parameter")
    fun contactLinkIsAppendedAsQueryParameter() {
        val parameterAndPrefixCode = errorLinkText + errorCode
        val errorLink = HybridPageElement(
                webDesktopLocator = "//a[contains(@href,\"$parameterAndPrefixCode\")]",
                androidLocator = null,
                page = this,
                helpfulName = "error link"
        )
        Assert.assertNotNull(errorLink)
    }
}
