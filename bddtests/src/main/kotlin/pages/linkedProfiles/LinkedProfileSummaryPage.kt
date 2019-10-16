package pages.linkedProfiles

import models.linkedProfiles.LinkedProfileSummary
import net.thucydides.core.annotations.DefaultUrl
import org.openqa.selenium.By
import org.openqa.selenium.NotFoundException
import pages.HybridPageElement
import pages.HybridPageObject
import pages.navigation.HeaderNative
import pages.text

@DefaultUrl("http://web.local.bitraft.io:3000/linked-profiles/summary")
class LinkedProfileSummaryPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    private val dateOfBirth = HybridPageElement(
            webDesktopLocator = "//span[@id='user-date-of-birth']",
            page = this
    )

    private val nhsNumber = HybridPageElement(
            webDesktopLocator = "//span[@id='user-nhs-number']",
            page = this
    )

    private val gpPractice = HybridPageElement(
            webDesktopLocator = "//span[@id='user-gp-practice']",
            page = this
    )

    fun isLoaded(patientName: String) {
        headerNative.waitForPageHeaderText(patientName)
    }

    private fun isFeatureEnabled(elementSid: String): Boolean {
        val listItem = findByXpath("//li[@id='$elementSid']")

        val tickIcon = listItem.containsElements(By.cssSelector(".nhsuk-icon__tick"))
        val crossIcon = listItem.containsElements(By.cssSelector(".nhsuk-icon__cross"))

        return when {
            tickIcon -> true
            crossIcon -> false
            else -> throw NotFoundException("Expected tick or cross icon to be displayed")
        }
    }

    fun getDisplayedLinkedProfileDetail(): LinkedProfileSummary {
        val appointments = "book-an-appointment"
        val orderRepeatPrescription = "order-repeat-prescription"
        val viewMedicalRecord = "view-medical-record"

        val linkedProfileDetail = LinkedProfileSummary(
                dateOfBirth = dateOfBirth.text,
                nhsNumber = nhsNumber.text,
                gpPracticeName = gpPractice.text,
                canBookAppointment = isFeatureEnabled(appointments),
                canOrderRepeatPrescription = isFeatureEnabled(orderRepeatPrescription),
                canViewMedicalRecord = isFeatureEnabled(viewMedicalRecord)
        )

        return linkedProfileDetail
    }
}