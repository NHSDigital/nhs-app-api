package webdrivers

import org.openqa.selenium.chrome.ChromeOptions
import utils.GlobalSerenityHelpers
import utils.getOrNull


open class ChromeHeadlessDriver : ChromeDriver() {

    override fun configureOptions(): ChromeOptions {
        val options = ChromeOptions()
        val currentDir = System.getProperty("user.dir")
        val userAgent = GlobalSerenityHelpers.USER_AGENT.getOrNull<String>()
        val prefs: MutableMap<String, Any> = HashMap()
        prefs["download.default_directory"] = "$currentDir/tmpDownloads"
        prefs["download.prompt_for_download"] = false
        prefs["download.directory_upgrade"] =  true
        prefs["browser.setDownloadBehavior.behavior"] = "allow"
        prefs["browser.setDownloadBehavior.downloadPath"] =  "$currentDir/tmpDownloads"
        options.setExperimentalOption("prefs", prefs)
        options.addArguments("--disable-extensions")

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
