package features.serviceJourneyRules.factories

import constants.Supplier
import models.Patient
import org.junit.Assert
import utils.SerenityHelpers
import java.util.*

private const val ODSCODE_IM1_ECONSULT_OLC_DISABLED_NOMINATED_PHARMACY_ENABLED = "A11111"
private const val ODSCODE_INFORMATICA_NOMINATED_PHARMACY_DISABLED = "A22222"
private const val ODSCODE_GP_AT_HAND_CONFIGURATIONS = "A44444"
private const val ODSCODE_IM1_MESSAGING_DOCUMENTS_ENABLED = "A10003"
private const val ODSCODE_IM1_MESSAGING_DOCUMENTS_DISABLED = "A10004"
private const val TPP_ONLINE_CONSULTATIONS_DISABLED = "A55555"
private const val VISION_ONLINE_CONSULTATIONS_DISABLED = "A66666"
private const val EMIS_GP_MEDICAL_RECORD_V1 = "A80001"
private const val TPP_GP_MEDICAL_RECORD_V1 = "A80002"
private const val VISION_GP_MEDICAL_RECORD_V1 = "A80003"
private const val MICROTEST_GP_MEDICAL_RECORD_V1 = "A80004"

class ServiceJourneyRulesMapper {
    companion object {
        public val ODSCODE_HOME_SCREEN_PUBLIC_HEALTH_NOTIFICATION = "A10006"

        private val journeysToGpInformationMap = mapOf(
                GpInformation(Supplier.EMIS, ODSCODE_IM1_ECONSULT_OLC_DISABLED_NOMINATED_PHARMACY_ENABLED) to
                        EnumSet.of(JourneyType.APPOINTMENTS_IM1,
                                JourneyType.MEDICAL_RECORD_IM1,
                                JourneyType.PRESCRIPTIONS_IM1,
                                JourneyType.ONLINE_CONSULTATIONS_DISABLED,
                                JourneyType.NOMINATED_PHARMACY_ENABLED,
                                JourneyType.NOTIFICATIONS_DISABLED,
                                JourneyType.MESSAGES_DISABLED,
                                JourneyType.USER_INFO_DISABLED,
                                JourneyType.SILVER_INTEGRATION_CONSULTATIONS_PKB,
                                JourneyType.SILVER_INTEGRATION_MESSAGES_PKB,
                                JourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS_PKB),
                GpInformation(Supplier.EMIS, ODSCODE_INFORMATICA_NOMINATED_PHARMACY_DISABLED) to
                        EnumSet.of(JourneyType.APPOINTMENTS_INFORMATICA,
                                JourneyType.MEDICAL_RECORD_IM1,
                                JourneyType.PRESCRIPTIONS_IM1,
                                JourneyType.NOMINATED_PHARMACY_DISABLED),
                GpInformation(Supplier.EMIS, ODSCODE_GP_AT_HAND_CONFIGURATIONS) to
                        EnumSet.of(JourneyType.APPOINTMENTS_GPATHAND,
                                JourneyType.MEDICAL_RECORD_GPATHAND,
                                JourneyType.PRESCRIPTIONS_GPATHAND,
                                JourneyType.SILVER_INTEGRATION_CONSULTATIONS_NONE,
                                JourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_NONE,
                                JourneyType.SILVER_INTEGRATION_MESSAGES_NONE),
                GpInformation(Supplier.TPP, TPP_ONLINE_CONSULTATIONS_DISABLED) to
                        EnumSet.of(JourneyType.ONLINE_CONSULTATIONS_DISABLED,
                                JourneyType.NOTIFICATIONS_ENABLED,
                                JourneyType.MESSAGES_ENABLED,
                                JourneyType.USER_INFO_ENABLED),

            // Medical Record V1
            GpInformation(Supplier.VISION, VISION_ONLINE_CONSULTATIONS_DISABLED)
                to EnumSet.of(JourneyType.ONLINE_CONSULTATIONS_DISABLED),

            // Medical Record V1
            GpInformation(Supplier.EMIS, EMIS_GP_MEDICAL_RECORD_V1) to
                EnumSet.of(JourneyType.MEDICAL_RECORD_VERSION_1),
            GpInformation(Supplier.VISION, VISION_GP_MEDICAL_RECORD_V1) to
                EnumSet.of(JourneyType.MEDICAL_RECORD_VERSION_1),
            GpInformation(Supplier.MICROTEST, MICROTEST_GP_MEDICAL_RECORD_V1) to
                EnumSet.of(JourneyType.MEDICAL_RECORD_VERSION_1),
            GpInformation(Supplier.TPP, TPP_GP_MEDICAL_RECORD_V1) to
                EnumSet.of(JourneyType.MEDICAL_RECORD_VERSION_1),

            // Home Screen Public Health Notifications
            GpInformation(Supplier.EMIS, ODSCODE_HOME_SCREEN_PUBLIC_HEALTH_NOTIFICATION) to
                EnumSet.of(JourneyType.HOME_SCREEN_PUBLIC_HEALTH_NOTIFICATIONS),
            // Medical Record V1
            GpInformation(Supplier.EMIS, EMIS_GP_MEDICAL_RECORD_V1) to
                    EnumSet.of(JourneyType.MEDICAL_RECORD_VERSION_1),
            GpInformation(Supplier.VISION, VISION_GP_MEDICAL_RECORD_V1) to
                    EnumSet.of(JourneyType.MEDICAL_RECORD_VERSION_1),
            GpInformation(Supplier.MICROTEST, MICROTEST_GP_MEDICAL_RECORD_V1) to
                    EnumSet.of(JourneyType.MEDICAL_RECORD_VERSION_1),
            GpInformation(Supplier.TPP, TPP_GP_MEDICAL_RECORD_V1) to
                    EnumSet.of(JourneyType.MEDICAL_RECORD_VERSION_1),

            // Gp Medical Record Documents / Im1 Messaging
            GpInformation(Supplier.EMIS, ODSCODE_IM1_MESSAGING_DOCUMENTS_ENABLED) to
                    EnumSet.of(JourneyType.DOCUMENTS_ENABLED,
                            JourneyType.IM1_MESSAGING_ENABLED,
                            JourneyType.IM1_MESSAGING_CANDELETEMESSAGES_DISABLED,
                            JourneyType.IM1_MESSAGING_CANUPDATEREADSTATUS_DISABLED,
                            JourneyType.IM1_MESSAGING_REQUIRESDETAILSREQUEST_DISABLED),
            GpInformation(Supplier.EMIS, ODSCODE_IM1_MESSAGING_DOCUMENTS_DISABLED) to
                    EnumSet.of(JourneyType.DOCUMENTS_DISABLED,
                            JourneyType.IM1_MESSAGING_DISABLED,
                            JourneyType.IM1_MESSAGING_CANDELETEMESSAGES_DISABLED,
                            JourneyType.IM1_MESSAGING_CANUPDATEREADSTATUS_DISABLED,
                            JourneyType.IM1_MESSAGING_REQUIRESDETAILSREQUEST_DISABLED)
        )

        fun findPatientForConfiguration(gpSystem: Supplier?, journeyType:JourneyType): Patient {
            return findPatientForConfiguration(gpSystem, arrayListOf(journeyType))
        }

        fun findPatientForConfiguration(gpSystem: Supplier?, configurations: List<ServiceJourneyRulesConfiguration>):
                Patient {
            val journeyTypes = configurations.map { configuration -> configuration.toJourneyType() }
            return findPatientForConfiguration(gpSystem, journeyTypes)
        }

        private fun findPatientForConfiguration(gpSystem: Supplier?, journeyTypes: Collection<JourneyType>): Patient {
            val gpInformation = findGpInformation(gpSystem, journeyTypes)
            Assert.assertNotNull("""Test setup incorrect: Cannot find a matching ods code for system:
                    $gpSystem and odsCode: ${gpInformation?.odsCode}, with given configuration in SJR""",
                    gpInformation)
            val patient = Patient.getDefault(gpInformation!!.gpSupplier).copy(odsCode = gpInformation.odsCode)
            SerenityHelpers.setGpSupplier(gpInformation.gpSupplier)
            SerenityHelpers.setPatient(patient)
            return patient
        }

        fun findGpInformation(gpSystem: Supplier?, journeyTypes: Collection<JourneyType>): GpInformation? {
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

        data class GpInformation(val gpSupplier: Supplier, val odsCode: String)

        enum class JourneyType {
            APPOINTMENTS_GPATHAND,
            APPOINTMENTS_IM1,
            APPOINTMENTS_INFORMATICA,
            MEDICAL_RECORD_GPATHAND,
            MEDICAL_RECORD_IM1,
            MEDICAL_RECORD_VERSION_1,
            MESSAGES_DISABLED,
            MESSAGES_ENABLED,
            NOTIFICATIONS_DISABLED,
            NOTIFICATIONS_ENABLED,
            NOMINATED_PHARMACY_DISABLED,
            NOMINATED_PHARMACY_ENABLED,
            ONLINE_CONSULTATIONS_DISABLED,
            PRESCRIPTIONS_GPATHAND,
            PRESCRIPTIONS_IM1,
            SILVER_INTEGRATION_CONSULTATIONS_PKB,
            SILVER_INTEGRATION_CONSULTATIONS_NONE,
            SILVER_INTEGRATION_MESSAGES_PKB,
            SILVER_INTEGRATION_MESSAGES_NONE,
            SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS_PKB,
            SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_NONE,
            USER_INFO_DISABLED,
            USER_INFO_ENABLED,
            HOME_SCREEN_PUBLIC_HEALTH_NOTIFICATIONS,
            DOCUMENTS_ENABLED,
            DOCUMENTS_DISABLED,
            IM1_MESSAGING_ENABLED,
            IM1_MESSAGING_CANDELETEMESSAGES_ENABLED,
            IM1_MESSAGING_CANDELETEMESSAGES_DISABLED,
            IM1_MESSAGING_CANUPDATEREADSTATUS_ENABLED,
            IM1_MESSAGING_CANUPDATEREADSTATUS_DISABLED,
            IM1_MESSAGING_REQUIRESDETAILSREQUEST_ENABLED,
            IM1_MESSAGING_REQUIRESDETAILSREQUEST_DISABLED,
            IM1_MESSAGING_DISABLED
        }
    }
}

