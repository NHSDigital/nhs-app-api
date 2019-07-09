package mocking.stubs.myMedicalRecord

import mocking.MockingClient
import mocking.data.myrecord.MedicationsData
import mocking.emis.medications.EmisMedicationsBuilder
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment.Companion.TIMEOUT_DELAY
import mocking.stubs.EmisStubsPatientFactory.Companion.goodPatientEMIS
import mocking.stubs.EmisStubsPatientFactory.Companion.serviceNotEnabledPatientEMIS
import mocking.stubs.EmisStubsPatientFactory.Companion.timeoutPatientEMIS
import models.Patient
import java.time.Duration

class MedicationsStubs(private val mockingClient: MockingClient) {
    fun generateEMISStubs() {
        val medicationDataLoader = MedicationsData.getEmisMedicationData()
        val mapEMISMedicationsRequestStubs =
                InputResponse<Patient, EmisMedicationsBuilder>()
                        .addResponse(goodPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(medicationDataLoader)
                        }

                        .addResponse(serviceNotEnabledPatientEMIS) { builder
                            ->
                            builder.respondWithExceptionWhenNotEnabled()
                        }

                        .addResponse(timeoutPatientEMIS) { builder
                            ->
                            builder.respondWithSuccess(medicationDataLoader)
                                    .delayedBy(Duration.ofSeconds(TIMEOUT_DELAY))
                        }

        mapEMISMedicationsRequestStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis { scenario.getResponse(myRecord.medicationsRequest(scenario.forMatcher)) }
        }

    }
}