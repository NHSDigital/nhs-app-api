package mocking.defaults.dataPopulation.journies.courses

import mocking.defaults.dataPopulation.journies.prescriptions.PrescriptionsData
import mocking.emis.models.MedicationCourse
import mocking.emis.models.PrescriptionType
import java.util.*

object CoursesData {

    fun getCourseData(maxCourses: Int, numOfRepeats: Int, numCanBeRequested: Int, medicationCourses: MutableList<MedicationCourse>) : MutableList<MedicationCourse> {

        var numberOfRepeats = numOfRepeats;
        var numberCanBerequested = numCanBeRequested

        // Create courses first as these will be used in the prescriptions
        for (course in 1..maxCourses) {
            val constituents = mutableListOf<String>()
            for (constituentNo in 1..PrescriptionsData.getrandomNumber(5)) {
                constituents.add("Constituent" + constituentNo)
            }


            // Create a default course
            var createdCourse = MedicationCourse(UUID.randomUUID().toString(),
                    PrescriptionsData.getCourseName(),
                    PrescriptionsData.getDosage(),
                    PrescriptionsData.getQuatity(),
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
                numberCanBerequested = numberCanBerequested - 1
            }

            medicationCourses.add(createdCourse)
        }

        return medicationCourses
    }
}