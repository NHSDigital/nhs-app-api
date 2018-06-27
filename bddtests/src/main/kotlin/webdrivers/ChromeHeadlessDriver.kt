package webdrivers

import io.github.bonigarcia.wdm.WebDriverManager
import net.thucydides.core.webdriver.DriverSource
import org.openqa.selenium.WebDriver
import org.openqa.selenium.chrome.ChromeDriver
import org.openqa.selenium.chrome.ChromeOptions


open class ChromeHeadlessDriver : DriverSource {

    override fun newDriver(): WebDriver? {
        WebDriverManager.chromedriver().setup()
        val options = ChromeOptions()
            options.addArguments("--headless")
            options.addArguments("--disable-gpu")
            options.addArguments("--no-sandbox")
        return ChromeDriver(options)
    }

    override fun takesScreenshots(): Boolean {
        return true
    }
}
