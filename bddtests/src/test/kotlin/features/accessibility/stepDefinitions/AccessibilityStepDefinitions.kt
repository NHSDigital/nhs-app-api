package features.accesibility.stepDefinitions

import config.Config
import cucumber.api.java.Before
import cucumber.api.java.en.Then
import pages.HybridPageObject
import java.io.File

class AccessibilityStepDefinitions {
    lateinit var page : HybridPageObject
    private val outputFolder = "${Config.instance.accessibilityOutputFolder}"

    @Before("@accessibility")
    fun createOutputFolderIfRequired() {

        val folder = File(outputFolder)

        if (!folder.exists())
        {
            folder.mkdir()
        }
    }

    @Then("^the (\\w+) page is saved to disk$")
    fun savePageSourceToDisk(pageToSave: String) {

        val file = File("$outputFolder/$pageToSave.html")

        val driver = page.driver

        file.printWriter().use {out ->
            out.print(driver.pageSource)
        }
    }
}