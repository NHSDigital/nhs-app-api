package webdrivers

import io.github.bonigarcia.wdm.WebDriverManager
import net.thucydides.core.webdriver.DriverSource
import org.openqa.selenium.WebDriver
import org.openqa.selenium.chrome.ChromeDriver
import org.openqa.selenium.chrome.ChromeOptions
import org.openqa.selenium.remote.CapabilityType


open class ChromeHeadlessOWASPDriver : DriverSource {

    override fun newDriver(): WebDriver? {
        WebDriverManager.chromedriver().setup()
        val options = ChromeOptions()
        options.addArguments("--headless")
        options.addArguments("--disable-gpu")
        options.addArguments("--no-sandbox")

        val proxyAddress = "localhost:8266"
        options.addArguments("--proxy-server=" + proxyAddress )

        options.setCapability(CapabilityType.ACCEPT_SSL_CERTS, true)
        return ChromeDriver(options)
    }

    override fun takesScreenshots(): Boolean {
        return true
    }
}
