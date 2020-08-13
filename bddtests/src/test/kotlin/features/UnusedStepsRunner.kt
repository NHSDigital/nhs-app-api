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
        monochrome = true,
        plugin = ["unused:target/Unused-Steps.txt"]
)
class UnusedStepsRunner {
    @Managed
    lateinit var driver: WebDriver
}
