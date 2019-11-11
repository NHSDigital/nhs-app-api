package pages.linkedProfiles.shutterPages

import org.junit.Assert
import pages.text

class MedicalRecordShutterComponent : ShutterComponent() {

    fun assertSubHeaderText(patientName: String) {
        Assert.assertEquals(
                "Sub header text does not match",
                "You do not have access to $patientName's medical record",
                subHeaderText.text
        )
    }

    fun assertSummaryText(patientName: String) {
        Assert.assertEquals(
                "Summary text does not match",
                "Contact $patientName's GP surgery to request access.",
                summaryText.text
        )
    }

    fun assertSwitchText() {
        Assert.assertEquals(
                "Switch text does not match",
                "Switch to your profile to view your GP medical record.",
                switchText.textValue
        )
    }
}
