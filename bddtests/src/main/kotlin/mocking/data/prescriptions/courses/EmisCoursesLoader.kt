package mocking.data.prescriptions.courses

import mocking.data.prescriptions.EmisPrescriptionLoader
import models.prescriptions.MedicationCourse
import mocking.emis.models.PrescriptionType
import java.util.*

object EmisCoursesLoader: ICoursesLoader<MutableList<MedicationCourse>> {

    override lateinit var data:MutableList<MedicationCourse>

    override fun loadData(maxCourses: Int,
                      numOfRepeats: Int,
                      numCanBeRequested: Int,
                      //medicationCourses: MutableList<MedicationCourse>,
                      includeDosage: Boolean,
                      includeQuantity: Boolean) {

        var numberOfRepeats = numOfRepeats
        var numberCanBeRequested = numCanBeRequested

        var medicationCourses = mutableListOf<MedicationCourse>()

        // Create courses first as these will be used in the prescriptions
        for (course in 1..maxCourses) {
            val constituents = mutableListOf<String>()
            for (constituentNo in 1..EmisPrescriptionLoader.getRandomNumber(5)) {
                constituents.add("Constituent" + constituentNo)
            }

            // Create a default course
            val createdCourse = MedicationCourse(UUID.randomUUID().toString(),
                    EmisPrescriptionLoader.getCourseName(),
                    if (includeDosage) EmisPrescriptionLoader.getDosage() else null,
                    if (includeQuantity) EmisPrescriptionLoader.getQuantity() else null,
                    PrescriptionType.Acute,
                    constituents,
                    false)

            // Check if the course needs to be set to repeat
            if(numberOfRepeats != 0){
                createdCourse.prescriptionType = PrescriptionType.Repeat
                numberOfRepeats--
            }

            // Check if the course needs to be true for canBeRequested
            if(numberCanBeRequested != 0){
                createdCourse.canBeRequested = true
                numberCanBeRequested--
            }

            medicationCourses.add(createdCourse)
        }

        this.data = medicationCourses
    }

    override fun getAvailableCoursesFilteredSortedOrdered(): List<MedicationCourse> {
        var coursesDataFiltered = data.filter { medicationCourse -> medicationCourse.canBeRequested!! }.toMutableList()
        coursesDataFiltered = coursesDataFiltered.filter { medicationCourse -> medicationCourse.prescriptionType == PrescriptionType.Repeat }.toMutableList()
        coursesDataFiltered = coursesDataFiltered.sortedBy { medicationCourse -> medicationCourse.name }.toMutableList()
        coursesDataFiltered = coursesDataFiltered.take(100).toMutableList()

        return coursesDataFiltered
    }
}
