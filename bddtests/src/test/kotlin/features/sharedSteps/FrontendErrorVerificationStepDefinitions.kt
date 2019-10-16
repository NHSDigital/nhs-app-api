package features.sharedSteps

import cucumber.api.java.en.Then
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject

class FrontendErrorVerificationStepDefinitions : HybridPageObject() {

    @Then("^In the error message I see the service reference number prefix with \"([^\"]*)\"$")
    fun iSeeTheServiceReferenceNumberPrefix(prefix: String) {
            val errorCode = HybridPageElement(
                    webDesktopLocator = "//span[@id=\"errorCode\"]",
                    androidLocator = null,
                    page = this,
                    helpfulName = "error message"
            )
            Assert.assertEquals("the error code prefix does not match the expected prefix",
                    prefix.toLowerCase(), errorCode.textValue.substring(0,2).toLowerCase())
        }
    }