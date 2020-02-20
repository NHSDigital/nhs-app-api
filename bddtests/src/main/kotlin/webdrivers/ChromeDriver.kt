package webdrivers

import config.Config
import io.github.bonigarcia.wdm.WebDriverManager
import net.thucydides.core.webdriver.DriverSource
import org.openqa.selenium.WebDriver
import org.openqa.selenium.chrome.ChromeDriver
import org.openqa.selenium.chrome.ChromeOptions
import utils.GlobalSerenityHelpers
import utils.getOrNull
import webdrivers.options.ChromeOptionManager.Companion.DEBUG_PORT

private const val CHROME_PROCESS_NAME = "chrome"

open class ChromeDriver : DriverSource {

    override fun newDriver(): WebDriver? {

        if(Config.instance.isDockerised) {
            val chromeProcessList = "ps -ae".runCommand()
                    .split('\n')
                    .filter { it.contains(CHROME_PROCESS_NAME) }

            if (chromeProcessList.any()) {
                chromeProcessList.forEach { chromeProcess ->
                    println("Found a zombie chrome: $chromeProcess")
                    val pid = chromeProcess.trim(' ').split(" ").first()
                    println("Killing the process with pid: $pid")

                    "kill -9 $pid".runCommand()
                }
                println("Process list after kill is:")
                println("ps -ae".runCommand())
            }
        }

        WebDriverManager.chromedriver().setup()

        /**
        Configure the web socket debug port for communicating with the
        chrome instance.
        */
        return ChromeDriver(configureOptions()
                .addArguments("--remote-debugging-port=$DEBUG_PORT")
        )
    }

    open fun configureOptions(): ChromeOptions {
        val currentDir = System.getProperty("user.dir")
        val options = ChromeOptions()
        val prefs: MutableMap<String, Any> = HashMap()
        val userAgent = GlobalSerenityHelpers.USER_AGENT.getOrNull<String>()
        prefs["download.default_directory"] = "$currentDir/tmpDownloads"
        options.setExperimentalOption("prefs", prefs)
        options.addArguments("--disable-extensions")

        if(userAgent != null){
            options.addArguments("user-agent=$userAgent")
        }

        return options
    }

    override fun takesScreenshots(): Boolean {
        return true
    }

    private fun String.runCommand(): String {
        return ProcessBuilder(split(" "))
                .redirectOutput(ProcessBuilder.Redirect.PIPE)
                .redirectError(ProcessBuilder.Redirect.INHERIT)
                .start()
                .inputStream
                .bufferedReader()
                .readText()


    }

}
