package features.prescriptions.stepDefinitions

import constants.ErrorResponseCodeTpp
import constants.Supplier
import mocking.stubs.prescriptions.factories.PrescriptionsFactory
import mocking.MockingClient
import mocking.data.prescriptions.IPrescriptionLoader
import mocking.defaults.VisionMockDefaults
import mocking.tpp.models.Error
import models.Patient
import models.prescriptions.PrescriptionLoaderConfiguration
import utils.SerenityHelpers
import utils.getOrNull
import utils.set
import java.time.OffsetDateTime

private const val DELAY_IN_SECONDS = 31L
class PrescriptionsDataSetup {

    companion object {

        private val mockingClient = MockingClient.instance

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

            val prescriptionLoaderConfig = PrescriptionLoaderConfiguration(
                    numberOfPrescriptions, numOfCourses, numOfRepeats
            )

            prescriptionLoader.loadData(prescriptionLoaderConfig)

            PrescriptionsFactory.getForSupplier(SerenityHelpers.getGpSupplier())
                    .setupWiremockAndDataWithDelay(null, prescriptionLoader, fromdate, toDate)
        }

        fun setupWiremockAndDataWithDelay(fromdate: OffsetDateTime,
                                          toDate: OffsetDateTime,
                                          numberOfPrescriptions: Int,
                                          numOfRepeats: Int) {

            val prescriptionLoader = getPrescriptionLoader()

            val prescriptionLoaderConfig = PrescriptionLoaderConfiguration(
                   noPrescriptions = numberOfPrescriptions, noRepeats = numOfRepeats, noCourses = numOfRepeats
            )

            prescriptionLoader.loadData(prescriptionLoaderConfig)

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
                    mockingClient.forEmis.mock {
                                prescriptions.prescriptionsRequest(currentPatient).respondWithPrescriptionsNotEnabled()
                            }

                    mockingClient.forEmis.mock {
                                prescriptions.coursesRequest(currentPatient).respondWithPrescriptionsNotEnabled()
                            }
                }
                Supplier.TPP -> {
                    mockingClient.forTpp.mock {
                                prescriptions.listRepeatMedication(currentPatient)
                                        .respondWithError(
                                                Error(ErrorResponseCodeTpp.NO_ACCESS,
                                                        "Error Occurred",
                                                        "1f907c07-9063-4d3a-81d7-ee8c98c54f4a"))
                            }
                }
                Supplier.VISION -> {
                    mockingClient.forVision.mock {
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
