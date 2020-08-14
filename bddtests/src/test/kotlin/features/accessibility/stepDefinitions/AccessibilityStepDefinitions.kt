package features.accesibility.stepDefinitions

import config.Config
import io.cucumber.java.Before
import io.cucumber.java.en.Then
import org.openqa.selenium.JavascriptExecutor
import pages.HybridPageObject
import java.io.File

class AccessibilityStepDefinitions {
    lateinit var page: HybridPageObject
    private val outputFolder = Config.instance.accessibilityOutputFolder

    @Before("@accessibility")
    fun createOutputFolderIfRequired() {

        val folder = File(outputFolder)

        if (!folder.exists()) {
            folder.mkdir()
        }
    }

    @Then("^the (\\w+) page is saved to disk$")
    fun savePageSourceToDisk(pageToSave: String) {

        val file = File("$outputFolder/$pageToSave.html")

        val jsExecutor = page.driver as JavascriptExecutor

        var pageSource = jsExecutor.executeScript("return document.documentElement.outerHTML").toString()

        // NB: Remove HotJar script elements to prevent injection of duplicate HotJar elements during automated
        //     accessibility testing, which are reported as errors
        pageSource = removeHotJarScriptElements(pageSource)

        file.printWriter().use { out ->
            out.print(pageSource)
        }
    }

    private fun removeHotJarScriptElements(pageSource: String): String {

        val matchHotJarScriptElement =
            "(?:<script[^>]*>[^<]*?static.hotjar[^<]*?<\\/script>)|(?:<script[^>]*hotjar[^>]*><\\/script>)"

        return pageSource.replace(matchHotJarScriptElement.toRegex(), "")
    }
}
