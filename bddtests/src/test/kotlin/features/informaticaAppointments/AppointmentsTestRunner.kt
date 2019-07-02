package features.informaticaAppointments

import cucumber.api.CucumberOptions
import net.serenitybdd.cucumber.CucumberWithSerenity
import net.thucydides.core.annotations.Managed
import org.junit.runner.RunWith
import org.openqa.selenium.WebDriver

@RunWith(CucumberWithSerenity::class)
@CucumberOptions(
        features = ["src/test/kotlin/features/informaticaAppointments"],
        glue = ["features"]
)
class AppointmentsTestRunner {
    @Managed
    lateinit var driver: WebDriver
}