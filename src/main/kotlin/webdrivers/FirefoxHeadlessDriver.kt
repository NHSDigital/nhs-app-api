package webdrivers

import io.github.bonigarcia.wdm.WebDriverManager
import net.thucydides.core.webdriver.DriverSource
import org.openqa.selenium.WebDriver
import org.openqa.selenium.firefox.FirefoxDriver
import org.openqa.selenium.firefox.FirefoxOptions


open class FirefoxHeadlessDriver : DriverSource {

    override fun newDriver(): WebDriver? {
        WebDriverManager.firefoxdriver().setup()
        val options = FirefoxOptions()
            options.addArguments("--headless")
        return FirefoxDriver(options)
    }

    override fun takesScreenshots(): Boolean {
        return true
    }
}
