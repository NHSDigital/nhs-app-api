package mocking.stubs.prescriptions

import mocking.MockingClient
import mocking.data.prescriptions.EmisPrescriptionLoader
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.emis.prescriptions.EmisPrescriptionsBuilder
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment.Companion.TIMEOUT_DELAY
import mocking.stubs.StubsPatientFactory.Companion.goodPatientEMIS
import mocking.stubs.StubsPatientFactory.Companion.serviceNotEnabledPatientEMIS
import mocking.stubs.StubsPatientFactory.Companion.timeoutPatientEMIS
import models.Patient
import java.time.Duration

class ViewPrescriptionsStubs(private val mockingClient: MockingClient) {

    companion object {
        private const val NUMBER_OF_PRESCRIPTIONS = 5
        private const val NUMBER_OF_COURSES = 5
        private const val NUMBER_OF_REPEAT_PRESCRIPTIONS = 5
    }
    fun generateEMISStubs() {
        val loadEMISPrescriptions = prescriptionLoaderEMIS()
        val mapEMISViewPrescriptionRequestStubs =
                InputResponse<Patient, EmisPrescriptionsBuilder>()
                        .addResponse(goodPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(loadEMISPrescriptions)
                                    .whenScenarioStateIs("Started")
                        }

                        .addResponse(serviceNotEnabledPatientEMIS) { builder
                            ->
                            builder.respondWithPrescriptionsNotEnabled()
                        }

                        .addResponse(timeoutPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(loadEMISPrescriptions)
                                    .whenScenarioStateIs("Started")
                                    .delayedBy(Duration.ofSeconds(TIMEOUT_DELAY))
                        }

        mapEMISViewPrescriptionRequestStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis { scenario.getResponse(prescriptionsRequest(scenario.forMatcher)) }
        }
    }

    private fun prescriptionLoaderEMIS(): PrescriptionRequestsGetResponse {
        val prescriptionLoader = EmisPrescriptionLoader
        prescriptionLoader.loadData(
                noPrescriptions = NUMBER_OF_PRESCRIPTIONS,
                noCourses = NUMBER_OF_COURSES,
                noRepeats = NUMBER_OF_REPEAT_PRESCRIPTIONS,
                showDosage = true,
                showQuantity = true
        )

        return prescriptionLoader.data
    }
}