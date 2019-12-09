package webdrivers

import org.openqa.selenium.chrome.ChromeOptions
import utils.GlobalSerenityHelpers
import utils.getOrNull


open class ChromeHeadlessDriver : ChromeDriver() {

    override fun configureOptions(): ChromeOptions {
        val options = ChromeOptions()
        val userAgent = GlobalSerenityHelpers.USER_AGENT.getOrNull<String>()

        if(userAgent != null){
            options.addArguments("user-agent=$userAgent")
        }

        return options
                .addArguments("--headless")
                .addArguments("--disable-gpu")
                .addArguments("--no-sandbox")
                .addArguments("--window-size=1080,1920")
    }
}
