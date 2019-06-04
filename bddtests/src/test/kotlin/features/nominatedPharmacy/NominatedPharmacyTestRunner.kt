package features.nominatedPharmacy

import cucumber.api.CucumberOptions
import net.serenitybdd.cucumber.CucumberWithSerenity
import net.thucydides.core.annotations.Managed
import org.junit.runner.RunWith
import org.openqa.selenium.WebDriver

@RunWith(CucumberWithSerenity::class)
@CucumberOptions(
        features = ["src/test/kotlin/features/nominatedPharmacy"],
        glue = ["features"]
)
class NominatedPharmacyTestRunner {
    @Managed
    lateinit var driver: WebDriver
}