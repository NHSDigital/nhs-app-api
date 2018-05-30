package features.courses.steps

import mocking.emis.models.MedicationCourse
import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.RepeatPrescriptionsPage

open class CourseSteps {

    lateinit var repeatPrescriptions : RepeatPrescriptionsPage

    @Step
    open fun isLoaded() {
        repeatPrescriptions.shouldBeDisplayed()
    }

    fun assertCorrectRepeatPrescriptionsShown(coursesData: MutableList<MedicationCourse>) {
        repeatPrescriptions.checkVisiblePrescriptions(coursesData)
    }

}