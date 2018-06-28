package mocking.defaults.dataPopulation.journies.courses

import mocking.defaults.dataPopulation.journies.prescriptions.PrescriptionsData
import mocking.emis.models.MedicationCourse
import mocking.emis.models.PrescriptionType
import java.util.*

object CoursesData {

    fun getCourseData(maxCourses: Int,
                      numOfRepeats: Int,
                      numCanBeRequested: Int,
                      medicationCourses: MutableList<MedicationCourse>,
                      includeDosage: Boolean,
                      includeQuantity: Boolean) : MutableList<MedicationCourse> {

        var numberOfRepeats = numOfRepeats
        var numberCanBerequested = numCanBeRequested

        // Create courses first as these will be used in the prescriptions
        for (course in 1..maxCourses) {
            val constituents = mutableListOf<String>()
            for (constituentNo in 1..PrescriptionsData.getRandomNumber(5)) {
                constituents.add("Constituent" + constituentNo)
            }

            var prefix = ""
            if (!includeDosage) {
                prefix += "no-dosage "
            }
            if (!includeQuantity) {
                prefix += "no-quantity "
            }

            // Create a default course
            val createdCourse = MedicationCourse(UUID.randomUUID().toString(),
                    prefix + PrescriptionsData.getCourseName(),
                    if (includeDosage) PrescriptionsData.getDosage() else null,
                    if (includeQuantity) PrescriptionsData.getQuantity() else null,
                    PrescriptionType.Acute,
                    constituents,
                    false)

            // Check if the course needs to be set to repeat
            if(numberOfRepeats != 0){
                createdCourse.prescriptionType = PrescriptionType.Repeat
                numberOfRepeats--
            }

            // Check if the course needs to be true for canBeRequested
            if(numberCanBerequested != 0){
                createdCourse.canBeRequested = true
                numberCanBerequested--
            }

            medicationCourses.add(createdCourse)
        }

        return medicationCourses
    }
}