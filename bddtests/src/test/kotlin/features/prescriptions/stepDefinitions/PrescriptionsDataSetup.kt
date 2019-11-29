package features.prescriptions.stepDefinitions

import constants.ErrorResponseCodeTpp
import constants.Supplier
import features.prescriptions.factories.PrescriptionsFactory
import mocking.MockingClient
import mocking.data.prescriptions.IPrescriptionLoader
import mocking.defaults.VisionMockDefaults
import mocking.tpp.models.Error
import models.Patient
import utils.SerenityHelpers
import utils.getOrNull
import utils.set
import java.time.OffsetDateTime

private const val DELAY_IN_SECONDS = 31L
class PrescriptionsDataSetup {

    companion object {

        val mockingClient = MockingClient.instance

        fun initialize(gpSystem: Supplier) {
            PrescriptionsSerenityHelpers.PROVIDER.set(gpSystem)
            val existingPatient = SerenityHelpers.getPatientOrNull()
            if (existingPatient == null) {
                SerenityHelpers.setPatient(Patient.getDefault(gpSystem))
            }
            PrescriptionsSerenityHelpers.PRESCRIPTIONS_LOADER.set(IPrescriptionLoader.getPrescriptionsLoader(gpSystem))
        }

        fun setupWiremockAndData(fromdate: OffsetDateTime,
                                 toDate: OffsetDateTime,
                                 numOfCourses: Int,
                                 numOfRepeats: Int,
                                 numberOfPrescriptions: Int) {

            val prescriptionLoader = getPrescriptionLoader()

            prescriptionLoader.loadData(
                    numberOfPrescriptions,
                    numOfCourses,
                    numOfRepeats)

            PrescriptionsFactory.getForSupplier(SerenityHelpers.getGpSupplier())
                    .setupWiremockAndDataWithDelay(null, prescriptionLoader, fromdate, toDate)
        }

        fun setupWiremockAndDataWithDelay(fromdate: OffsetDateTime,
                                          toDate: OffsetDateTime,
                                          numberOfPrescriptions: Int,
                                          numOfRepeats: Int) {

            val prescriptionLoader = getPrescriptionLoader()

            prescriptionLoader.loadData(
                    numberOfPrescriptions,
                    numOfRepeats,
                    numOfRepeats)

            PrescriptionsFactory.getForSupplier(SerenityHelpers.getGpSupplier())
                    .setupWiremockAndDataWithDelay(DELAY_IN_SECONDS, prescriptionLoader, fromdate, toDate)
        }

        private fun getPrescriptionLoader(): IPrescriptionLoader<*> {
            var prescriptionLoader = PrescriptionsSerenityHelpers
                    .PRESCRIPTIONS_LOADER.getOrNull<IPrescriptionLoader<*>>()
            if (prescriptionLoader == null) {
                val gpSystem = SerenityHelpers.getGpSupplier()
                initialize(gpSystem)
                prescriptionLoader = PrescriptionsSerenityHelpers.PRESCRIPTIONS_LOADER
                        .getOrNull<IPrescriptionLoader<*>>()
            }
            return prescriptionLoader!!
        }

        fun disabled(currentPatient: Patient, currentProvider: Supplier) {
            when (currentProvider) {
                Supplier.EMIS -> {
                    mockingClient
                            .forEmis {
                                prescriptions.prescriptionsRequest(currentPatient).respondWithPrescriptionsNotEnabled()
                            }

                    mockingClient
                            .forEmis {
                                prescriptions.coursesRequest(currentPatient).respondWithPrescriptionsNotEnabled()
                            }
                }
                Supplier.TPP -> {
                    mockingClient
                            .forTpp {
                                prescriptions.listRepeatMedication(currentPatient)
                                        .respondWithError(
                                                Error(ErrorResponseCodeTpp.NO_ACCESS,
                                                        "Error Occurred",
                                                        "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                            }
                }
                Supplier.VISION -> {
                    mockingClient
                            .forVision {
                                authentication.getConfigurationRequest(
                                        VisionMockDefaults.visionUserSessionPrescriptionDisabled)
                                        .respondWithSuccess(VisionMockDefaults
                                                .visionConfigurationResponsePrescriptionsDisabled)
                            }
                }
                else -> {
                    throw NotImplementedError("Invalid GP System")
                }
            }
        }
    }
}