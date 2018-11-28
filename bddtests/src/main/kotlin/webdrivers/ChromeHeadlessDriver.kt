package webdrivers

import org.openqa.selenium.chrome.ChromeOptions


open class ChromeHeadlessDriver : ChromeDriver() {

    override fun configureOptions(): ChromeOptions {
        return ChromeOptions()
                .addArguments("--headless")
                .addArguments("--disable-gpu")
                .addArguments("--no-sandbox")
                .addArguments("--window-size=1080,1920")
    }
}
