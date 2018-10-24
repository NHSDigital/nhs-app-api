package mocking.data.prescriptions.courses

import constants.SerenitySessionKeys.Companion.PRESCRIPTION_COMMENTS_ALLOWED
import mocking.data.prescriptions.EmisPrescriptionLoader
import mocking.gpServiceBuilderInterfaces.courses.ICoursesLoader
import mocking.vision.models.EligibleRepeats
import mocking.vision.models.RepeatCourse
import models.prescriptions.MedicationCourse
import net.serenitybdd.core.Serenity
import org.joda.time.DateTime
import java.util.*
import kotlin.collections.ArrayList

object VisionCoursesLoader: ICoursesLoader<EligibleRepeats> {
    override lateinit var data: EligibleRepeats
    private const val COURSE_ID_MAX = 5000
    private const val COURSES_MAX = 100

    override fun loadData(maxCourses: Int,
                          numOfRepeats: Int,
                          numCanBeRequested: Int,
                          includeDosage: Boolean,
                          includeQuantity: Boolean) {

        var numberOfRepeats = numOfRepeats

        val higherNumber: Int

        higherNumber = if (maxCourses > numOfRepeats) {
            maxCourses
        } else {
            numberOfRepeats
        }

        var repeats = ArrayList<RepeatCourse>()

        // Create courses first as these will be used in the prescriptions
        for (course in 1..higherNumber) {

            // Create a default course
            val createdCourse = RepeatCourse(
                    (1..COURSE_ID_MAX).random().toString(),
                    EmisPrescriptionLoader.getCourseName(),
                    if (includeDosage) EmisPrescriptionLoader.getDosage() else null,
                    if (includeQuantity) EmisPrescriptionLoader.getQuantity(course) else null,
                    DateTime.now().minusDays(course).toLocalDateTime().toString()
                  )

            repeats.add(createdCourse)
        }

        var eligibleRepeats = EligibleRepeats()

        var allowFreeText = true
        if (Serenity.hasASessionVariableCalled(PRESCRIPTION_COMMENTS_ALLOWED)) {
            allowFreeText = Serenity.sessionVariableCalled<Boolean>(PRESCRIPTION_COMMENTS_ALLOWED)
        }

        eligibleRepeats.settings.allowFreetext = allowFreeText
        eligibleRepeats.repeat = repeats

        this.data = eligibleRepeats
    }

    fun IntRange.random() =
            Random().nextInt((endInclusive + 1) - start) +  start

    override fun getAvailableCoursesFilteredSortedOrdered(): List<MedicationCourse> {

        var courses = data.repeat

        courses = courses.sortedBy { medicationCourse -> medicationCourse.drug }.toMutableList()
        courses = courses.take(COURSES_MAX).toMutableList()

        return courses.map { m -> MedicationCourse(m.getRepeatCourseId()!!, m.drug!!, m.dosage, m.quantity) }
    }
}
