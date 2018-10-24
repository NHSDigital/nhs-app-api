package mocking.gpServiceBuilderInterfaces.courses

import models.prescriptions.MedicationCourse

interface ICoursesLoader<T> {
    var data: T

    fun loadData(maxCourses: Int,
                 numOfRepeats: Int,
                 numCanBeRequested: Int,
                 includeDosage: Boolean,
                 includeQuantity: Boolean)

    fun getAvailableCoursesFilteredSortedOrdered(): List<MedicationCourse>
}
