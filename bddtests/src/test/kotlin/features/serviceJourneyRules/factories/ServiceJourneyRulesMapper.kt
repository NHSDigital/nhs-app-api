package features.serviceJourneyRules.factories

import models.Patient
import org.junit.Assert
import utils.SerenityHelpers
import java.util.*

private const val EMIS_GP_SUPPLIER = "EMIS"
private const val TPP_GP_SUPPLIER = "TPP"
private const val VISION_GP_SUPPLIER = "VISION"
private const val ODSCODE_IM1_ECONSULT_OLC_DISABLED_NOMINATED_PHARMACY_ENABLED = "A11111"
private const val ODSCODE_INFORMATICA_NOMINATED_PHARMACY_DISABLED = "A22222"
private const val ODSCODE_GP_AT_HAND_CONFIGURATIONS = "A44444"
private const val TPP_ONLINE_CONSULTATIONS_DISABLED = "A55555"
private const val VISION_ONLINE_CONSULTATIONS_DISABLED = "A66666"

class ServiceJourneyRulesMapper {

    companion object {
        private val journeysToGpInformationMap = mapOf(
                GpInformation(EMIS_GP_SUPPLIER, ODSCODE_IM1_ECONSULT_OLC_DISABLED_NOMINATED_PHARMACY_ENABLED) to
                        EnumSet.of(JourneyType.APPOINTMENTS_IM1,
                                JourneyType.MEDICAL_RECORD_IM1,
                                JourneyType.PRESCRIPTIONS_IM1,
                                JourneyType.ONLINE_CONSULTATIONS_DISABLED,
                                JourneyType.NOMINATED_PHARMACY_ENABLED,
                                JourneyType.NOTIFICATIONS_DISABLED),
                GpInformation(EMIS_GP_SUPPLIER, ODSCODE_INFORMATICA_NOMINATED_PHARMACY_DISABLED) to
                        EnumSet.of(JourneyType.APPOINTMENTS_INFORMATICA,
                                JourneyType.MEDICAL_RECORD_IM1,
                                JourneyType.PRESCRIPTIONS_IM1,
                                JourneyType.NOMINATED_PHARMACY_DISABLED),
                GpInformation(EMIS_GP_SUPPLIER, ODSCODE_GP_AT_HAND_CONFIGURATIONS) to
                        EnumSet.of(JourneyType.APPOINTMENTS_GPATHAND,
                                JourneyType.MEDICAL_RECORD_GPATHAND,
                                JourneyType.PRESCRIPTIONS_GPATHAND),
                GpInformation(TPP_GP_SUPPLIER, TPP_ONLINE_CONSULTATIONS_DISABLED) to
                        EnumSet.of(JourneyType.ONLINE_CONSULTATIONS_DISABLED,
                                JourneyType.NOTIFICATIONS_ENABLED),
                GpInformation(VISION_GP_SUPPLIER, VISION_ONLINE_CONSULTATIONS_DISABLED) to
                        EnumSet.of(JourneyType.ONLINE_CONSULTATIONS_DISABLED)
        )

        fun findPatientForConfiguration(gpSystem: String?, configurations: List<ServiceJourneyRulesConfiguration>):
                Patient {
            val journeyTypes =
                    configurations.map { configuration -> configuration.toJourneyType() }
            val gpInformation = findGpInformation(gpSystem, journeyTypes)

            Assert.assertNotNull("Test setup incorrect: Cannot find a matching ods code for system:"
                    + gpSystem + "and odsCode: " + gpInformation?.odsCode + ", with given configuration in SJR",
                    gpInformation)

            val patient = Patient.getDefault(gpInformation!!.gpSupplier).copy(odsCode = gpInformation.odsCode)

            SerenityHelpers.setGpSupplier(gpInformation.gpSupplier)
            SerenityHelpers.setPatient(patient)

            return patient
        }

        private fun findGpInformation(gpSystem: String?, journeyTypes: Collection<JourneyType>): GpInformation? {
            val filteredMappings =
                    if (gpSystem != null)
                        journeysToGpInformationMap.filter { map -> map.key.gpSupplier == gpSystem }
                    else journeysToGpInformationMap

            filteredMappings
                    .forEach { (gpInformation, journeyTypesConfig) ->
                        if (journeyTypesConfig.size >= journeyTypes.size &&
                                journeyTypesConfig.containsAll(journeyTypes)) {
                            return gpInformation
                        }
                    }
            return null
        }

        data class GpInformation(val gpSupplier: String, val odsCode: String)

        enum class JourneyType {
            APPOINTMENTS_GPATHAND,
            APPOINTMENTS_IM1,
            APPOINTMENTS_INFORMATICA,
            MEDICAL_RECORD_GPATHAND,
            MEDICAL_RECORD_IM1,
            NOTIFICATIONS_DISABLED,
            NOTIFICATIONS_ENABLED,
            NOMINATED_PHARMACY_DISABLED,
            NOMINATED_PHARMACY_ENABLED,
            ONLINE_CONSULTATIONS_DISABLED,
            PRESCRIPTIONS_GPATHAND,
            PRESCRIPTIONS_IM1,
        }
    }
}

