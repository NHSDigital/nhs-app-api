package pages

import mocking.emis.models.MedicationCourse
import net.thucydides.core.annotations.DefaultUrl
import net.thucydides.core.pages.WrongPageError
import org.junit.Assert
import pages.navigation.Header
import java.util.function.Consumer

@DefaultUrl("http://localhost:3000/prescriptions/repeat-courses")
open class RepeatPrescriptionsPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    var headerText: String = "Select medication"
    lateinit var headerBar: Header

    override fun shouldBeDisplayed() {
        if(!headerBar.isVisible(headerText)){
            throw WrongPageError("The expected header is not visible, you are on the wrong page.")
        }

        super.shouldBeDisplayed()
    }

    fun checkVisiblePrescriptions(coursesData: MutableList<MedicationCourse>) {
        var nameXpath = "//label[@for='prescription-selection']"
        var instructionsXpath = "//span[@aria-label=\"prescription-description\"]"

        var namesListed = findAllByXpath(nameXpath)
        var instructionsListed = findAllByXpath(instructionsXpath)

        for (i in coursesData.indices){
            var name = coursesData[i].name
            var instructions = coursesData[i].dosage + " - " + coursesData[i].quantityRepresentation.replace("  ", " ")

            var nameAtIndex = namesListed[i].text
            var instructionAtIndex = instructionsListed[i].text

            Assert.assertEquals("Unexpected medication name at index " + i, name, nameAtIndex)
            Assert.assertEquals("Unexpected instructions at index " + i, instructions, instructionAtIndex)
        }
    }

}
