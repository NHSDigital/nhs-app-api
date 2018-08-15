package features.courses.steps

import models.prescriptions.MedicationCourse
import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.prescription.RepeatPrescriptionsPage

open class CourseSteps {

    lateinit var repeatPrescriptions : RepeatPrescriptionsPage

    @Step
    open fun isLoaded() {
        repeatPrescriptions.shouldBeDisplayed()
    }

    fun assertCorrectRepeatPrescriptionsShown(coursesData: List<MedicationCourse>) {
        repeatPrescriptions.verifyVisiblePrescriptions(coursesData)
    }

    fun assertCorrectRepeatPrescriptionsSelected(courses: List<MedicationCourse>) {
        for (course in courses) {
            repeatPrescriptions.verifyPrescriptionIsSelected(course)
        }
    }

    fun selectRepeatPrescriptions(coursesToSelect: List<MedicationCourse>) {
        for (course in coursesToSelect) {
            repeatPrescriptions.selectRepeatPrescription(course)
        }
    }

    @Step
    fun assertNoRepeatPrescriptionsSelectedMessageShown() {
        Assert.assertTrue(repeatPrescriptions.isNoRepeatPrescriptionsSelectedMessageVisible())
    }

    @Step
    fun assertNoMedicationAvailableToOrderMessageShown() {
        Assert.assertTrue(repeatPrescriptions.isNoMedicationAvailableToOrderMessageVisible())
    }
}