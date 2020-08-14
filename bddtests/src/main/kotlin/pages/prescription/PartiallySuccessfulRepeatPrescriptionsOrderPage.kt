package pages.prescription

import mockingFacade.prescriptions.PartialSuccessFacade
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject

@DefaultUrl("http://web.local.bitraft.io:3000/prescriptions/repeat-partial-success")
open class PartiallySuccessfulRepeatPrescriptionsOrderPage : HybridPageObject() {
    val title by lazy {
        HybridPageElement(
                "//h1[normalize-space(text())='Part of your prescription has not been ordered']",
                "//h1[normalize-space(text())='Part of your prescription has not been ordered']",
                null,
                null,
                this,
                helpfulName = "header")
    }

    override fun shouldBeDisplayed() {
        title.waitForElement(HEADER_RETRIES)
        super.shouldBeDisplayed()
    }

    fun verifyMedications(partialSuccess: PartialSuccessFacade) {
        val displayedUnsuccessfulItems =
                findByXpath("//ul[contains(@class, 'nhsuk-list--cross')]").thenFindAll("li")
        val displayedSuccessfulItems =
                findByXpath("//ul[contains(@class, 'nhsuk-list--tick')]").thenFindAll("li")

        Assert.assertEquals(
                "Unexpected number of unsuccessful medications",
                partialSuccess.unsuccessfulMedications.size,
                displayedUnsuccessfulItems.size)
        Assert.assertEquals(
                "Unexpected number of successful medications",
                partialSuccess.successfulMedications.size,
                displayedSuccessfulItems.size)

        for (i in partialSuccess.unsuccessfulMedications.indices) {
            val expectedMedicationName = partialSuccess.unsuccessfulMedications[i]
            val medicationNameOnScreen = displayedUnsuccessfulItems[i]

            Assert.assertEquals(
                    "Expected medication name does not match on screen text",
                    expectedMedicationName,
                    medicationNameOnScreen.text)
        }

        for (i in partialSuccess.successfulMedications.indices) {
            val expectedMedicationName = partialSuccess.successfulMedications[i]
            val medicationNameOnScreen = displayedSuccessfulItems[i]

            Assert.assertEquals(
                    "Expected medication name does not match on screen text",
                    expectedMedicationName,
                    medicationNameOnScreen.text)
        }
    }
}
