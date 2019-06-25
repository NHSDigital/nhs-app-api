package features.serviceJourneyRules

import cucumber.api.CucumberOptions
import net.serenitybdd.cucumber.CucumberWithSerenity
import net.thucydides.core.annotations.Managed
import org.junit.runner.RunWith
import org.openqa.selenium.WebDriver

@RunWith(CucumberWithSerenity::class)
@CucumberOptions(features = ["src/test/kotlin/features/serviceJourneyRules"], glue = ["features"])
class ServiceJourneyRulesTestRunner {

    @Managed
    lateinit var driver: WebDriver
}
