package mocking.defaults.dataPopulation.journies.prescriptions

import mocking.MockingClient
import mocking.data.prescriptions.MicrotestPrescriptionLoader
import mocking.data.prescriptions.VisionPrescriptionLoader
import mocking.defaults.VisionMockDefaults
import models.Patient
import models.prescriptions.PrescriptionLoaderConfiguration

private const val SEVEN_PRESCRIPTIONS = 7
private const val SEVEN_COURSES = 7
private const val SEVEN_REPEATS = 7

private const val ONE_PRESCRIPTION = 1
private const val ONE_COURSE = 1
private const val ONE_REPEAT = 1

class PrescriptionsHistoryJourney(private val client: MockingClient) {

    fun createFor(patient: Patient) {
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


        val prescriptionLoaderConfig = PrescriptionLoaderConfiguration(
                noPrescriptions = 1,noCourses = 1, noRepeats = 1
        )

                VisionPrescriptionLoader.loadData(prescriptionLoaderConfig)
        client
                .forVision {
                    prescriptions.getPrescriptionHistoryRequest(VisionMockDefaults.getVisionUserSession(patient))
                            .respondWithSuccess(VisionPrescriptionLoader.data)
                }

        MicrotestPrescriptionLoader.loadData(prescriptionLoaderConfig)
        client
                .forMicrotest {
                    prescriptions.getPrescriptionHistoryRequest(patient)
                            .respondWithSuccess(MicrotestPrescriptionLoader.data)
                }

    }
}
