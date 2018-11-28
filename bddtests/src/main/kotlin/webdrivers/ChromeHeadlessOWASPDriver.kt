package webdrivers

import org.openqa.selenium.chrome.ChromeOptions
import org.openqa.selenium.remote.CapabilityType


open class ChromeHeadlessOWASPDriver : ChromeDriver() {

    override fun configureOptions(): ChromeOptions {
        val options = ChromeOptions()
        options.setCapability(CapabilityType.ACCEPT_SSL_CERTS, true)
        return options
                .addArguments("--headless")
                .addArguments("--disable-gpu")
                .addArguments("--no-sandbox")
                .addArguments("--proxy-server=localhost:8266")
    }
}
