package pages.prescription

import mocking.emis.models.MedicationCourse
import net.serenitybdd.core.annotations.findby.By
import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.annotations.findby.How
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import net.thucydides.core.pages.WrongPageError
import org.junit.Assert
import org.openqa.selenium.Keys
import pages.HybridPageObject
import pages.navigation.Header
import pages.HybridPageObject.Companion.PageType

@DefaultUrl("http://localhost:3000/prescriptions/repeat-courses")
open class RepeatPrescriptionsPage : HybridPageObject(PageType.WEBVIEW_APP) {

    var headerText: String = "Confirm Prescription"
    lateinit var headerBar: Header

    @FindBy(how = How.ID, using = "specialRequest")
    lateinit var specialRequestTextArea: WebElementFacade

    @FindBy(how = How.ID, using = "btn_order_prescription")
    lateinit var orderPrescriptionButton: WebElementFacade

    override fun shouldBeDisplayed() {
        if(!headerBar.isVisible(headerText)){
            throw WrongPageError("The expected header is not visible, you are on the wrong page.")
        }

        super.shouldBeDisplayed()
    }

    fun isNoRepeatPrescriptionsSelectedMessageVisible(): Boolean {
        val message = "Select at least one medicine"
        return findByXpath("//p[contains(.,'$message')]").isVisible
    }

    fun verifyVisiblePrescriptions(coursesData: List<MedicationCourse>) {
        var nameXpath = "//label[contains(@for, 'prescription-')]"
        var instructionsXpath = "//span[@aria-label=\"prescription-description\"]"

        var namesListed = findAllByXpath(nameXpath)
        var instructionsListed = findAllByXpath(instructionsXpath)

        for (i in coursesData.indices) {
            var name = coursesData[i].name
            var instructions = coursesData[i].dosage + " - " + coursesData[i].quantityRepresentation.replace("  ", " ")

            var nameAtIndex = namesListed[i].text
            var instructionAtIndex = instructionsListed[i].text

            Assert.assertEquals("Unexpected medication name at index $i", name, nameAtIndex)
            Assert.assertEquals("Unexpected instructions at index $i", instructions, instructionAtIndex)
        }
    }

    fun selectXPrescriptionsToOrder(numberOfSubscriptionsToSelect: Int) {
        val repeatPrescriptionContainers = findAllByXpath("//div[@data-purpose='repeat-prescription']")

        for (i in 0..(numberOfSubscriptionsToSelect-1)) {
            var label = repeatPrescriptionContainers[i].findElement(By.tagName( "label"))
            label.click()
        }
    }

    fun clickConfirmAndOrderRepeatSubscriptionButton() {
        var confirmAndOrderRepeatButtonLocator = "//button[contains(text(), 'Confirm and order prescription')]"
        var confirmAndOrderRepeatButton = findByXpath(confirmAndOrderRepeatButtonLocator)
        confirmAndOrderRepeatButton.click()
    }

    fun selectRepeatPrescription(courseToSelect: MedicationCourse) {
        val repeatPrescriptionContainers = findAllByXpath("//div[@data-purpose='repeat-prescription']")

        repeatPrescriptionContainers.forEach( { el ->
            var nameOnScreen = el.findElement(By.tagName( "label"))
            var instructionsOnScreen = el.findElement(By.tagName("span"))
            var inputElement = el.findElement(By.cssSelector("input[name=prescription]"))

            if (courseToSelect.name == nameOnScreen.text
                    && courseToSelect.getInstructionsText() == instructionsOnScreen.text
                    && !inputElement.isSelected) {
                nameOnScreen.click()
                return
            }
        })

        Assert.fail("Didn't find medication course with: \nname: ${courseToSelect.name} \ndosage: ${courseToSelect.getInstructionsText()}")
    }

    fun verifyPrescriptionIsSelected(medicationCourse: MedicationCourse) {
        val repeatPrescriptionContainers = findAllByXpath("//div[@data-purpose='repeat-prescription']")

        repeatPrescriptionContainers.forEach( { el ->
            var nameOnScreen = el.findElement(By.tagName( "label"))
            var instructionsOnScreen = el.findElement(By.tagName("span"))
            var inputElement = el.findElement(By.tagName("input"))

            if (medicationCourse.name == nameOnScreen.text
                    && medicationCourse.getInstructionsText() == instructionsOnScreen.text) {
                Assert.assertEquals(medicationCourse.medicationCourseGuid, inputElement.getAttribute("value"))
                Assert.assertTrue(inputElement.isSelected())
                return
            }
        })

        Assert.fail("Didn't find medication course with: \nname: ${medicationCourse.name} \ndosage: ${medicationCourse.getInstructionsText()}")
    }

    fun clickContinueButton() {
        orderPrescriptionButton.sendKeys(Keys.ENTER)
    }

    fun typeTextIntoSpecialRequestTextArea(text: String) {
        typeInto(specialRequestTextArea, text)
    }
}
