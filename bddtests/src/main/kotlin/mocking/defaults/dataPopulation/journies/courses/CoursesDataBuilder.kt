package mocking.defaults.dataPopulation.journies.courses

import mocking.defaults.dataPopulation.journies.prescriptions.PrescriptionsData
import mocking.emis.models.PrescriptionType
import models.prescriptions.MedicationCourse
import java.util.*

private const val MAX_PRESCRIPTIONS_NUMBER = 5
class CoursesDataBuilder {

    private var maxCourses: Int = 0
    private var numOfRepeats: Int = 0
    private var numCanBeRequested: Int = 0
    private var medicationCourses: MutableList<MedicationCourse> = mutableListOf()
    private var includeDosage: Boolean = false
    private var includeQuantity: Boolean = false

    fun maxCourses(value: Int): CoursesDataBuilder {
        maxCourses = value
        return this
    }

    fun numOfRepeats(value: Int): CoursesDataBuilder {
        numOfRepeats = value
        return this
    }

    fun numCanBeRequested(value: Int): CoursesDataBuilder {
        numCanBeRequested = value
        return this
    }

    fun medicationCourses(value: MutableList<MedicationCourse>): CoursesDataBuilder {
        medicationCourses = value
        return this
    }

    fun includeDosage(value: Boolean): CoursesDataBuilder {
        includeDosage = value
        return this
    }

    fun includeQuantity(value: Boolean): CoursesDataBuilder {
        includeQuantity = value
        return this
    }

    fun build(): MutableList<MedicationCourse> {
        return createData()
    }


    private fun createData(): MutableList<MedicationCourse> {
        var numberOfRepeats = numOfRepeats
        var numberCanBeRequested = numCanBeRequested

        // Create courses first as these will be used in the prescriptions
        for (course in 1..maxCourses) {
            val constituents = mutableListOf<String>()
            for (constituentNo in 1..PrescriptionsData.getRandomNumber(MAX_PRESCRIPTIONS_NUMBER)) {
                constituents.add("Constituent" + constituentNo)
            }

            val prefix = getPrefix(includeDosage, includeQuantity)

            // Create a default course
            val createdCourse = MedicationCourse(UUID.randomUUID().toString(),
                    prefix + PrescriptionsData.getCourseName(),
                    if (includeDosage) PrescriptionsData.getDosage() else null,
                    if (includeQuantity) PrescriptionsData.getQuantity(course) else null,
                    PrescriptionType.Acute,
                    constituents,
                    false)

            // Check if the course needs to be set to repeat
            if (numberOfRepeats != 0) {
                createdCourse.prescriptionType = PrescriptionType.Repeat
                numberOfRepeats--
            }

            // Check if the course needs to be true for canBeRequested
            if (numberCanBeRequested != 0) {
                createdCourse.canBeRequested = true
                numberCanBeRequested--
            }

            medicationCourses.add(createdCourse)
        }

        return medicationCourses
    }

    private fun getPrefix(includeDosage: Boolean, includeQuantity: Boolean): String {
        var prefix = ""
        if (!includeDosage) {
            prefix += "no-dosage"
        }
        if (!includeQuantity) {
            prefix += "no-quantity"
        }
        return prefix
    }
}
