package mocking.data.prescriptions.courses

import mocking.data.prescriptions.EmisPrescriptionLoader
import mocking.emis.models.PrescriptionType
import mocking.gpServiceBuilderInterfaces.Courses.ICoursesLoader
import mocking.vision.models.EligableRepeats
import mocking.vision.models.Repeat
import mocking.vision.models.RepeatCourse
import mocking.vision.models.Settings
import models.prescriptions.MedicationCourse
import org.joda.time.DateTime
import java.text.SimpleDateFormat
import java.time.OffsetDateTime
import java.time.format.DateTimeFormatter
import java.util.*
import kotlin.collections.ArrayList

object VisionCoursesLoader: ICoursesLoader<EligableRepeats> {
    override lateinit var data: EligableRepeats
    private const val CONSTITUENTS_NUMBER = 5
    private const val COURSES_NUMBER = 100
    private const val COURSE_ID_MAX = 5000

    override fun loadData(maxCourses: Int,
                          numOfRepeats: Int,
                          numCanBeRequested: Int,
                          includeDosage: Boolean,
                          includeQuantity: Boolean) {

        var numberOfRepeats = numOfRepeats


        var eligableRepeats = EligableRepeats()

        eligableRepeats.settings = Settings(true)

        val higherNumber: Int

        higherNumber = if (maxCourses > numOfRepeats) {
            maxCourses
        } else {
            numberOfRepeats
        }

        var request = ArrayList<RepeatCourse>()

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

            request.add(createdCourse)
        }

        this.data = EligableRepeats(settings = Settings(true), request = request)
    }

    fun IntRange.random() =
            Random().nextInt((endInclusive + 1) - start) +  start

    override fun getAvailableCoursesFilteredSortedOrdered(): List<MedicationCourse> {

        return emptyList()
    }
}
