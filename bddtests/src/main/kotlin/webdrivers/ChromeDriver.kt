package webdrivers

import io.github.bonigarcia.wdm.WebDriverManager
import net.thucydides.core.webdriver.DriverSource
import org.openqa.selenium.WebDriver
import org.openqa.selenium.chrome.ChromeDriver
import org.openqa.selenium.chrome.ChromeOptions
import webdrivers.options.ChromeOptionManager.Companion.DEBUG_PORT

private const val LATEST_STABLE_CHROME_DRIVER_MAJOR_VERSION_NUMBER = "73.0.3683.68"

open class ChromeDriver : DriverSource {

    override fun newDriver(): WebDriver? {
        WebDriverManager.chromedriver().version(LATEST_STABLE_CHROME_DRIVER_MAJOR_VERSION_NUMBER).setup()

        /**
        Configure the web socket debug port for communicating with the
        chrome instance.
        */
        return ChromeDriver(configureOptions()
                .addArguments("--remote-debugging-port=$DEBUG_PORT")
        )
    }

    open fun configureOptions(): ChromeOptions {
        return ChromeOptions()
    }

    override fun takesScreenshots(): Boolean {
        return true
    }
}
