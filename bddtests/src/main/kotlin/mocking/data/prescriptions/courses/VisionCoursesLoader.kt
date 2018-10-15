package mocking.data.prescriptions.courses

import mocking.data.prescriptions.EmisPrescriptionLoader
import mocking.gpServiceBuilderInterfaces.Courses.ICoursesLoader
import mocking.vision.models.EligibleRepeats
import mocking.vision.models.RepeatCourse
import mocking.vision.models.Settings
import models.prescriptions.MedicationCourse
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


        var eligibleRepeats = EligibleRepeats()

        eligibleRepeats.settings = Settings(true)

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

        this.data = EligibleRepeats(settings = Settings(true), repeat = repeats)
    }

    fun IntRange.random() =
            Random().nextInt((endInclusive + 1) - start) +  start

    override fun getAvailableCoursesFilteredSortedOrdered(): List<MedicationCourse> {

        var courses = data.repeat!!

        courses = courses.sortedBy { medicationCourse -> medicationCourse.drug }.toMutableList()
        courses = courses.take(COURSES_MAX).toMutableList()

        return courses.map { m -> MedicationCourse(m.getRepeatCourseId()!!, m.drug!!, m.dosage, m.quantity) }
    }
}
