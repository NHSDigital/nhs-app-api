package features.pushNotifications

import cucumber.api.CucumberOptions
import net.serenitybdd.cucumber.CucumberWithSerenity
import net.thucydides.core.annotations.Managed
import org.junit.runner.RunWith
import org.openqa.selenium.WebDriver

@RunWith(CucumberWithSerenity::class)
@CucumberOptions(features = ["src/test/kotlin/features/pushNotifications"], glue = ["features"])
class PushNotificationsTestRunner {

    @Managed
    lateinit var driver: WebDriver
}