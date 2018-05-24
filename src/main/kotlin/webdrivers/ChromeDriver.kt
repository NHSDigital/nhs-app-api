package webdrivers

import net.thucydides.core.webdriver.DriverSource
import org.openqa.selenium.WebDriver
import org.openqa.selenium.chrome.ChromeDriver


open class ChromeDriver : DriverSource {

    override fun newDriver(): WebDriver? {
        return ChromeDriver()
    }

    override fun takesScreenshots(): Boolean {
        return true
    }
}
