package features.dataSharing

import cucumber.api.CucumberOptions
import net.serenitybdd.cucumber.CucumberWithSerenity
import net.thucydides.core.annotations.Managed
import org.junit.runner.RunWith
import org.openqa.selenium.WebDriver

@RunWith(CucumberWithSerenity::class)
@CucumberOptions(features = ["src/test/kotlin/features/dataSharing"], glue = ["features"])
class DataSharingTestRunner {

    @Managed
    lateinit var driver: WebDriver

}