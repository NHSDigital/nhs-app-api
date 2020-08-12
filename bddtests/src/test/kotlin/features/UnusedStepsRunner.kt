package features

import io.cucumber.junit.CucumberOptions
import net.serenitybdd.cucumber.CucumberWithSerenity
import net.thucydides.core.annotations.Managed
import org.junit.runner.RunWith
import org.openqa.selenium.WebDriver

@RunWith(CucumberWithSerenity::class)
@CucumberOptions(
        features = ["src/test/kotlin/features"],
        glue = ["features"],
        dryRun = true,
        plugin = ["unused:target/unused.log"]
)
class UnusedStepsRunner {
    @Managed
    lateinit var driver: WebDriver
}
