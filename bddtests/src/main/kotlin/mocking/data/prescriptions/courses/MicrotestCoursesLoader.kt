package mocking.data.prescriptions.courses

import mocking.data.prescriptions.MicrotestPrescriptionLoader
import mocking.gpServiceBuilderInterfaces.courses.ICoursesLoader
import mocking.microtest.prescriptions.Course
import mocking.microtest.prescriptions.CourseStatus
import models.prescriptions.MedicationCourse
import java.util.UUID

object MicrotestCoursesLoader: ICoursesLoader<MutableList<Course>> {

    override lateinit var data: MutableList<Course>

    private const val COURSES_NUMBER = 100

    override fun loadData(maxCourses: Int,
                          numOfRepeats: Int,
                          numCanBeRequested: Int,
                          includeDosage: Boolean,
                          includeQuantity: Boolean) {

        val medicationCourses = mutableListOf<Course>()

        // Create courses first as these will be used in the prescriptions
        for (course in 1..maxCourses) {
            // Create a default course
            val createdCourse = Course(
                    UUID.randomUUID().toString(),
                    MicrotestPrescriptionLoader.getCourseName(),
                    if (includeDosage) MicrotestPrescriptionLoader.getDosage() else null,
                    if (includeQuantity) MicrotestPrescriptionLoader.getQuantity(course) else null,
                    CourseStatus.Repeat)

            medicationCourses.add(createdCourse)
        }

        this.data = medicationCourses
    }

    override fun getAvailableCoursesFilteredSortedOrdered(): List<MedicationCourse> {
        var coursesDataFiltered = data
                .sortedBy { medicationCourse -> medicationCourse.name }
                .toMutableList()
        coursesDataFiltered = coursesDataFiltered.take(COURSES_NUMBER).toMutableList()

        return coursesDataFiltered.map { m -> MedicationCourse(m.id, m.name, m.dosage, m.quantity) }
    }
}
