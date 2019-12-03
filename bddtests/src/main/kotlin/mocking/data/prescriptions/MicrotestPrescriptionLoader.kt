package mocking.data.prescriptions

import mocking.microtest.prescriptions.Course
import mocking.microtest.prescriptions.PrescriptionCourse
import mocking.microtest.prescriptions.PrescriptionHistoryGetResponse
import mocking.microtest.prescriptions.PrescriptionStatus
import mocking.microtest.prescriptions.PrescriptionType
import models.prescriptions.PrescriptionLoaderConfiguration
import java.time.OffsetDateTime

object MicrotestPrescriptionLoader : IPrescriptionLoader<PrescriptionHistoryGetResponse> {

    override lateinit var data: PrescriptionHistoryGetResponse

    override fun loadData(prescriptionLoaderConfig: PrescriptionLoaderConfiguration,
                          prescriptionCompletedByProxy: Boolean) {

        val medicationCourses = mutableListOf<PrescriptionCourse>()

        if (prescriptionLoaderConfig.noPrescriptions > 0) {
            for (i in 1..prescriptionLoaderConfig.noPrescriptions) {
                val medication = PrescriptionCourse(
                        i.toString(),
                        OffsetDateTime.now().toString(),
                        PrescriptionStatus.Requested,
                        getCourseName(),
                        getQuantity(i),
                        getDosage(),
                        PrescriptionType.Repeat.toString(),
                        ""
                )

                medicationCourses.add(medication)
            }
        }

        data = PrescriptionHistoryGetResponse(medicationCourses)
    }

    fun orderCourses(orderedCourses: MutableList<Course>,
                     oldPrescriptionHistory: PrescriptionHistoryGetResponse = MicrotestPrescriptionLoader.data)
            : PrescriptionHistoryGetResponse {

        // Combine "just ordered" courses and existing courses so prescription history is reflective of current state.
        val allCourses = mutableListOf<PrescriptionCourse>()
        orderedCourses.forEach { course ->
            allCourses.add(
                    PrescriptionCourse(
                            course.id,
                            OffsetDateTime.now().toString(),
                            PrescriptionStatus.Requested,
                            course.name,
                            course.quantity!!,
                            course.dosage!!,
                            PrescriptionType.Repeat.toString(),
                            ""))
        }

        allCourses.addAll(oldPrescriptionHistory.courses)

        return PrescriptionHistoryGetResponse(allCourses.toList())
    }
}
