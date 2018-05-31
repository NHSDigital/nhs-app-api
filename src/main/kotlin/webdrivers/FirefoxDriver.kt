package webdrivers

import io.github.bonigarcia.wdm.WebDriverManager
import net.thucydides.core.webdriver.DriverSource
import org.openqa.selenium.WebDriver
import org.openqa.selenium.firefox.FirefoxDriver
import org.openqa.selenium.firefox.FirefoxOptions


open class FirefoxDriver : DriverSource {

    override fun newDriver(): WebDriver? {
        WebDriverManager.firefoxdriver().setup()
        return FirefoxDriver()
    }

    override fun takesScreenshots(): Boolean {
        return true
    }
}
