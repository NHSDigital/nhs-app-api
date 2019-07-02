package features.im1Appointments

import cucumber.api.CucumberOptions
import net.serenitybdd.cucumber.CucumberWithSerenity
import net.thucydides.core.annotations.Managed
import org.junit.runner.RunWith
import org.openqa.selenium.WebDriver

@RunWith(CucumberWithSerenity::class)
@CucumberOptions(
        features = ["src/test/kotlin/features/im1Appointments"],
        glue = ["features"]
)
class AppointmentsTestRunner {
    @Managed
    lateinit var driver: WebDriver
}