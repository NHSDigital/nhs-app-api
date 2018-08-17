package pages.prescription

import models.prescriptions.MedicationCourse
import net.serenitybdd.core.annotations.findby.By
import net.thucydides.core.annotations.DefaultUrl
import net.thucydides.core.pages.WrongPageError
import org.junit.Assert
import pages.HybridPageObject
import pages.navigation.Header
import pages.HybridPageObject.Companion.PageType
import pages.HybridPageElement


fun resolveDetailsField(dosage: String?, quantity: String?): String {

    if (dosage != null && quantity != null) {
        return dosage + " - " + quantity.replace("  ", " ")
    }
    else if (dosage != null) {
        return dosage
    }
    else if (quantity != null) {
        return quantity
    }
    return ""
}

@DefaultUrl("http://localhost:3000/prescriptions/repeat-courses")
open class RepeatPrescriptionsPage : HybridPageObject(PageType.WEBVIEW_APP) {

    var headerText: String = "Select medication"
    lateinit var headerBar: Header
    val PrescriptionInstructionsLocator = By.cssSelector("[data-label='prescription-description']")

    val orderRepeatPrescriptionButton = HybridPageElement(
            browserLocator = "//button[@id='btn_order_prescription']",
            androidLocator = null,
            page = this
    )

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

    fun isNoMedicationAvailableToOrderMessageVisible(): Boolean {
        val message = "You don't have any medication available to order right now"
        return findByXpath("//h3[contains(., \"$message\")]").isVisible
    }

    fun verifyVisiblePrescriptions(coursesData: List<MedicationCourse>) {
        val nameXpath = "//label[contains(@for, 'prescription-')]"
        val instructionsXpath = "//p[@data-label=\"prescription-description\"]"

        val namesListed = findAllByXpath(nameXpath)
        val instructionsListed = findAllByXpath(instructionsXpath)

        for (i in coursesData.indices) {
            val name = coursesData[i].name
            val instructions = resolveDetailsField(coursesData[i].dosage, coursesData[i].quantityRepresentation)

            val nameAtIndex = namesListed[i].text
            val instructionAtIndex = instructionsListed[i].text

            Assert.assertEquals("Unexpected medication name at index $i", name, nameAtIndex)
            Assert.assertEquals("Unexpected instructions at index $i", instructions, instructionAtIndex)
        }
    }

    fun selectXPrescriptionsToOrder(numberOfSubscriptionsToSelect: Int) {
        val repeatPrescriptionContainers = findAllByXpath("//div[@data-purpose='repeat-prescription']")

        for (i in 0..(numberOfSubscriptionsToSelect-1)) {
            val label = repeatPrescriptionContainers[i].findElement(By.tagName( "label"))
            label.click()
        }
    }

    fun clickConfirmAndOrderRepeatSubscriptionButton() {
        val confirmAndOrderRepeatButtonLocator = "//button[contains(text(), 'Confirm and order prescription')]"
        val confirmAndOrderRepeatButton = findByXpath(confirmAndOrderRepeatButtonLocator)
        confirmAndOrderRepeatButton.click()
    }

    fun selectRepeatPrescription(courseToSelect: MedicationCourse) {
        var prescription = getRepeatPrescription(courseToSelect)
        Assert.assertNotNull("Didn't find medication course with: \nname: ${courseToSelect.name} \ndosage: ${courseToSelect.getInstructionsText()}", prescription)
        prescription.element.click()
   }

    private fun getRepeatPrescription(courseToSelect: MedicationCourse):HybridPageElement {
        return HybridPageElement(
                browserLocator = "//div[p[text()[contains(.,'${courseToSelect.getInstructionsText()}')]]]/div/label[text()[contains(.,'${courseToSelect.name}')]]",
                androidLocator = null,
                page = this
        )
    }

    fun verifyPrescriptionIsSelected(medicationCourse: MedicationCourse) {
        val repeatPrescriptionContainers = findAllByXpath("//div[@data-purpose='repeat-prescription']")

        repeatPrescriptionContainers.forEach { el ->
            val nameOnScreen = el.findElement(By.tagName( "label"))
            val instructionsOnScreen = el.findElement(PrescriptionInstructionsLocator)
            val inputElement = el.findElement(By.tagName("input"))

            if (medicationCourse.name == nameOnScreen.text
                    && medicationCourse.getInstructionsText() == instructionsOnScreen.text) {
                Assert.assertEquals(medicationCourse.medicationCourseGuid, inputElement.getAttribute("value"))
                Assert.assertTrue(inputElement.isSelected())
                return
            }
        }

        Assert.fail("Didn't find medication course with: \nname: ${medicationCourse.name} \ndosage: ${medicationCourse.getInstructionsText()}")
    }

    fun clickContinueButton() {
        orderRepeatPrescriptionButton.element.click()
    }

    fun typeTextIntoSpecialRequestTextArea(text: String) {
        findByXpath("//*[@id='specialRequest']").sendKeys(text)
    }
}
