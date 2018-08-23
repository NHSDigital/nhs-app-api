package mocking.defaults.dataPopulation.journies.prescriptions

import mocking.MockingClient
import mocking.emis.models.PrescriptionRequestsGetResponse
import models.Patient

private const val SEVEN_PRESCRIPTIONS = 7
private const val SEVEN_COURSES = 7
private const val SEVEN_REPEATS = 7

private const val ONE_PRESCRIPTION = 1
private const val ONE_COURSE = 1
private const val ONE_REPEAT = 1

class PrescriptionsHistoryJourney(private val client: MockingClient) {

    fun createFor(patient: Patient) {
        val prescriptions = listOf(
                PrescriptionsData.loadPrescriptionsData(SEVEN_PRESCRIPTIONS, SEVEN_COURSES, SEVEN_REPEATS),
                PrescriptionsData.loadPrescriptionsData(ONE_PRESCRIPTION, ONE_COURSE, ONE_REPEAT, false),
                PrescriptionsData.loadPrescriptionsData(ONE_PRESCRIPTION, ONE_COURSE, ONE_REPEAT, true, false),
                PrescriptionsData.loadPrescriptionsData(ONE_PRESCRIPTION, ONE_COURSE, ONE_REPEAT, false, false)
        )

        client
                .forEmis {
                    prescriptionsRequest(patient)
                            .respondWithSuccess(
                                    PrescriptionsData.addResponses(prescriptions)
                            )
                }
    }
}
