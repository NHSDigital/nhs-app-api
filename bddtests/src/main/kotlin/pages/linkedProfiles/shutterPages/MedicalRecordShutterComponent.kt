package pages.linkedProfiles.shutterPages

import models.Patient
import pages.HybridPageObject
import pages.sharedElements.expectedPage.ExpectedPageStructure
import pages.sharedElements.expectedPage.ParsedPage
import pages.sharedElements.expectedPage.ExpectedPageStructureAssertor

class MedicalRecordShutterComponent :  HybridPageObject() {

    fun assertText(patient: Patient, patientDisplayName: String) {
        val firstLine = "You do not have access to $patientDisplayName's medical record"
        val expected = ExpectedPageStructure()
                .paragraph("Name")
                .paragraph(patient.formattedFullName())
                .paragraph("Age")
                .paragraph(patient.formattedAge())
                .paragraph(firstLine)
                .paragraph("Contact $patientDisplayName's GP surgery to request access.")
                .paragraph("Switch to your profile to view your GP medical record.")
                .button("Switch to my profile")
        val parsedPage = ParsedPage.parse(this,
                "//div[div/div/div/div/p/b[normalize-space(text())=\"$firstLine\"]]")
        ExpectedPageStructureAssertor().assert(parsedPage, expected.build())
    }
}
