package webdrivers

import org.openqa.selenium.chrome.ChromeOptions


open class ChromeNonGPUDriver : ChromeDriver() {

    override fun configureOptions(): ChromeOptions {
        return ChromeOptions()
            .addArguments("--disable-gpu")
            .addArguments("--no-sandbox")
    }
}
