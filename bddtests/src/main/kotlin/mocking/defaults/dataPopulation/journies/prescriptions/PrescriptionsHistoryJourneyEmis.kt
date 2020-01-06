package mocking.defaults.dataPopulation.journies.prescriptions

import models.Patient

private const val SEVEN_PRESCRIPTIONS = 7
private const val SEVEN_COURSES = 7
private const val SEVEN_REPEATS = 7

private const val ONE_PRESCRIPTION = 1
private const val ONE_COURSE = 1
private const val ONE_REPEAT = 1
class PrescriptionsHistoryJourneyEmis: PrescriptionsHistoryJourney() {

    override fun createFor(patient: Patient) {

        val prescriptionsToCreate = listOf(
                PrescriptionsData.loadPrescriptionsData(SEVEN_PRESCRIPTIONS, SEVEN_COURSES, SEVEN_REPEATS),
                PrescriptionsData.loadPrescriptionsData(ONE_PRESCRIPTION, ONE_COURSE, ONE_REPEAT, false),
                PrescriptionsData.loadPrescriptionsData(ONE_PRESCRIPTION, ONE_COURSE, ONE_REPEAT, true, false),
                PrescriptionsData.loadPrescriptionsData(ONE_PRESCRIPTION, ONE_COURSE, ONE_REPEAT, false, false)
        )
        client
                .forEmis {
                    prescriptions.prescriptionsRequest(patient)
                            .respondWithSuccess(PrescriptionsData.addResponses(prescriptionsToCreate))
                }
    }
}