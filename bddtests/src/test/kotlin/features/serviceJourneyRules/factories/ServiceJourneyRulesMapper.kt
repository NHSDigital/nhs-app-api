package features.serviceJourneyRules.factories

import constants.Supplier
import models.IdentityProofingLevel
import models.Patient
import org.junit.Assert
import utils.SerenityHelpers
import java.util.*

class ServiceJourneyRulesMapper {
    companion object {
        const val ODSCODE_HOME_SCREEN_PUBLIC_HEALTH_NOTIFICATION = "A10006"

        private val journeysToGpInformationMap = ServiceJourneyRulesConfigurationBuilder()
            .add(
                Supplier.EMIS, "A11111",
                EnumSet.of(
                    SJRJourneyType.APPOINTMENTS_IM1,
                    SJRJourneyType.CORONAVIRUS_INFORMATION_ENABLED,
                    SJRJourneyType.MEDICAL_RECORD_IM1,
                    SJRJourneyType.MESSAGES_DISABLED,
                    SJRJourneyType.NOMINATED_PHARMACY_ENABLED,
                    SJRJourneyType.NOTIFICATIONS_DISABLED,
                    SJRJourneyType.ONLINE_CONSULTATIONS_DISABLED,
                    SJRJourneyType.PRESCRIPTIONS_IM1,
                    SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_ADMIN_NONE,
                    SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_NONE,
                    SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_LIBRARY_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_MEDICINES_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_MESSAGES_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_RECORD_SHARING_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_TEST_RESULTS_PKB,
                    SJRJourneyType.USER_INFO_DISABLED
                )
            )
            .add(
                Supplier.EMIS, "A22222",
                EnumSet.of(
                    SJRJourneyType.APPOINTMENTS_INFORMATICA,
                    SJRJourneyType.MEDICAL_RECORD_IM1,
                    SJRJourneyType.NOMINATED_PHARMACY_DISABLED,
                    SJRJourneyType.PRESCRIPTIONS_IM1,
                    SJRJourneyType.SILVER_INTEGRATION_MEDICINES_NONE,
                    SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS,
                    SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_NONE,
                    SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_NONE
                )
            )
            .add(
                Supplier.EMIS, "A44444",
                EnumSet.of(
                    SJRJourneyType.APPOINTMENTS_GPATHAND,
                    SJRJourneyType.MEDICAL_RECORD_GPATHAND,
                    SJRJourneyType.NOTIFICATIONS_ENABLED,
                    SJRJourneyType.NOTIFICATION_PROMPT_DISABLED,
                    SJRJourneyType.PRESCRIPTIONS_GPATHAND,
                    SJRJourneyType.SILVER_INTEGRATION_ACCOUNT_ADMIN_NONE,
                    SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_NONE,
                    SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_NONE,
                    SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_NONE,
                    SJRJourneyType.SILVER_INTEGRATION_LIBRARY_NONE,
                    SJRJourneyType.SILVER_INTEGRATION_MESSAGES_NONE,
                    SJRJourneyType.SILVER_INTEGRATION_PARTICIPATION_NONE,
                    SJRJourneyType.SILVER_INTEGRATION_RECORD_SHARING_NONE,
                    SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_NONE,
                    SJRJourneyType.SILVER_INTEGRATION_TEST_RESULTS_NONE,
                    SJRJourneyType.SILVER_INTEGRATION_VACCINE_RECORD_NETCOMPANY_DISABLED,
                    SJRJourneyType.SILVER_INTEGRATION_VACCINE_RECORD_NETCOMPANY_P5_DISABLED,
                    SJRJourneyType.SILVER_INTEGRATION_VACCINE_RECORD_NHSD_DISABLED
                )
            )
            .add(
                Supplier.TPP, "A55555",
                EnumSet.of(
                    SJRJourneyType.IM1_MESSAGING_ENABLED,
                    SJRJourneyType.MESSAGES_ENABLED,
                    SJRJourneyType.NOTIFICATIONS_ENABLED,
                    SJRJourneyType.NOTIFICATION_PROMPT_ENABLED,
                    SJRJourneyType.ONLINE_CONSULTATIONS_DISABLED,
                    SJRJourneyType.ONLINE_CONSULTATIONS_DISABLED,
                    SJRJourneyType.SILVER_INTEGRATION_MESSAGES_PKB,
                    SJRJourneyType.USER_INFO_ENABLED
                )
            )
            .add(
                Supplier.VISION, "A66666",
                EnumSet.of(SJRJourneyType.ONLINE_CONSULTATIONS_DISABLED)
            )
            .add(
                Supplier.EMIS, "A80001",
                EnumSet.of(
                    SJRJourneyType.IM1_MESSAGING_ENABLED,
                    SJRJourneyType.IM1_MESSAGING_CANDELETEMESSAGES_DISABLED,
                    SJRJourneyType.IM1_MESSAGING_CANUPDATEREADSTATUS_ENABLED,
                    SJRJourneyType.IM1_MESSAGING_REQUIRESDETAILSREQUEST_ENABLED,
                    SJRJourneyType.MEDICAL_RECORD_VERSION_1,
                    SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_ENGAGE,
                    SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_ADMIN_ENGAGE,
                    SJRJourneyType.SILVER_INTEGRATION_MESSAGES_ENGAGE
                )
            )
            .add(
                Supplier.VISION, "A80003",
                EnumSet.of(SJRJourneyType.MEDICAL_RECORD_VERSION_1)
            )
            .add(
                Supplier.TPP, "A80002",
                EnumSet.of(
                    SJRJourneyType.MEDICAL_RECORD_VERSION_1,
                    SJRJourneyType.IM1_MESSAGING_CANDELETEMESSAGES_DISABLED,
                    SJRJourneyType.IM1_MESSAGING_CANUPDATEREADSTATUS_ENABLED,
                    SJRJourneyType.IM1_MESSAGING_ENABLED,
                    SJRJourneyType.IM1_MESSAGING_REQUIRESDETAILSREQUEST_DISABLED
                )
            )
            .add(
                Supplier.EMIS, ODSCODE_HOME_SCREEN_PUBLIC_HEALTH_NOTIFICATION,
                EnumSet.of(SJRJourneyType.HOME_SCREEN_PUBLIC_HEALTH_NOTIFICATIONS)
            )
            .add(
                Supplier.EMIS, "A10003",
                EnumSet.of(
                    SJRJourneyType.DOCUMENTS_ENABLED,
                    SJRJourneyType.IM1_MESSAGING_CANDELETEMESSAGES_DISABLED,
                    SJRJourneyType.IM1_MESSAGING_CANUPDATEREADSTATUS_DISABLED,
                    SJRJourneyType.IM1_MESSAGING_ENABLED,
                    SJRJourneyType.IM1_MESSAGING_REQUIRESDETAILSREQUEST_DISABLED,
                    SJRJourneyType.SILVER_INTEGRATION_MESSAGES_GNCR,
                    SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_GNCR,
                    SJRJourneyType.SILVER_INTEGRATION_ACCOUNT_ADMIN_GNCR
                )
            )
            .add(
                Supplier.EMIS, "A10004",
                EnumSet.of(
                    SJRJourneyType.DOCUMENTS_DISABLED,
                    SJRJourneyType.IM1_MESSAGING_CANDELETEMESSAGES_DISABLED,
                    SJRJourneyType.IM1_MESSAGING_CANUPDATEREADSTATUS_DISABLED,
                    SJRJourneyType.IM1_MESSAGING_DISABLED,
                    SJRJourneyType.IM1_MESSAGING_REQUIRESDETAILSREQUEST_DISABLED
                )
            )
            .add(
                Supplier.EMIS, "A00002",
                EnumSet.of(
                    SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_LIBRARY_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_MESSAGES_PKB_TESTSILVERTHIRDPARTYPROVIDER,
                    SJRJourneyType.SILVER_INTEGRATION_RECORD_SHARING_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_VACCINE_RECORD_NETCOMPANY_ENABLED,
                    SJRJourneyType.SILVER_INTEGRATION_VACCINE_RECORD_NETCOMPANY_P5_ENABLED,
                    SJRJourneyType.SILVER_INTEGRATION_VACCINE_RECORD_NHSD_ENABLED
                )
            )
            .add(
                Supplier.EMIS, "A80005",
                EnumSet.of(
                    SJRJourneyType.CORONAVIRUS_INFORMATION_DISABLED,
                    SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_LIBRARY_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_MESSAGES_PKB_TESTSILVERTHIRDPARTYPROVIDER,
                    SJRJourneyType.SILVER_INTEGRATION_RECORD_SHARING_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS_PKB
                )
            )
            .add(
                Supplier.EMIS, "A80006",
                EnumSet.of(
                    SJRJourneyType.NDOP_DISABLED,
                    SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_LIBRARY_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_MESSAGES_PKB_TESTSILVERTHIRDPARTYPROVIDER,
                    SJRJourneyType.SILVER_INTEGRATION_RECORD_SHARING_PKB,
                    SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS_PKB
                )
            )
            .add(
                Supplier.EMIS, "A80007",
                EnumSet.of(
                    SJRJourneyType.ONE_ONE_ONE_DISABLED,
                    SJRJourneyType.MEDICAL_RECORD_IM1
                )
            )
            .add(
                Supplier.EMIS, "B86013",
                EnumSet.of(
                    SJRJourneyType.MEDICAL_RECORD_IM1
                )
            )
            .add(
                Supplier.EMIS, "A82010",
                EnumSet.of(
                    SJRJourneyType.SILVER_INTEGRATION_ACCOUNT_ADMIN_SUBSTRAKT,
                    SJRJourneyType.SILVER_INTEGRATION_MESSAGES_SUBSTRAKT,
                    SJRJourneyType.SILVER_INTEGRATION_PARTICIPATION_SUBSTRAKT
                )
            )
            .add(
                Supplier.EMIS, "A10001",
                EnumSet.of(
                    SJRJourneyType.MEDICAL_RECORD_IM1
                )
            )
            .add(
                Supplier.EMIS, "W00001",
                EnumSet.of(
                    SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_WELLNESS_AND_PREVENTION
                )
            )
            .add(
                    Supplier.EMIS, "WF0001",
                    EnumSet.of(
                            SJRJourneyType.WAYFINDER_ENABLED
                    )
            )
            .add(
                Supplier.EMIS, "F00013",
                EnumSet.of(
                    SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_ACCURX,
                    SJRJourneyType.SILVER_INTEGRATION_MESSAGES_ACCURX
                    )
            )

        fun findPatientForConfiguration(
            gpSystem: Supplier?,
            journeyType: SJRJourneyType,
            proofLevel: IdentityProofingLevel? = null
        ): Patient {
            return findPatientForConfiguration(gpSystem, arrayListOf(journeyType), proofLevel)
        }

        fun findUniquePatientForConfiguration(
            gpSystem: Supplier?,
            journeyType: SJRJourneyType,
            proofLevel: IdentityProofingLevel? = null,
            setSerenityVariable: Boolean = true
        ): Patient {
            return findUniquePatientForConfiguration(
                gpSystem,
                arrayListOf(journeyType),
                proofLevel,
                setSerenityVariable
            )
        }

        fun findPatientForConfiguration(
            gpSystem: Supplier?,
            journeyTypes: Collection<SJRJourneyType>,
            proofLevel: IdentityProofingLevel? = null
        ): Patient {
            val gpInformation = journeysToGpInformationMap.find(gpSystem, journeyTypes)
            var patient = Patient.getDefault(gpInformation.gpSupplier).copy(odsCode = gpInformation.odsCode)
            if (proofLevel != null) {
                patient = patient.copy(identityProofingLevel = proofLevel)
            }

            SerenityHelpers.setGpSupplier(gpInformation.gpSupplier)
            SerenityHelpers.setPatient(patient)

            return patient
        }

        fun findUniquePatientForConfiguration(
            gpSystem: Supplier?,
            journeyTypes: Collection<SJRJourneyType>,
            proofLevel: IdentityProofingLevel? = null,
            setSerenityVariable: Boolean = true
        ): Patient {
            val gpInformation = journeysToGpInformationMap.find(gpSystem, journeyTypes)
            var patient = Patient.getDefault(gpInformation.gpSupplier).copy(
                odsCode = gpInformation.odsCode,
                subject = UUID.randomUUID().toString()
            )
            if (proofLevel != null) {
                patient = patient.copy(identityProofingLevel = proofLevel)
            }
            if (setSerenityVariable) {
                SerenityHelpers.setGpSupplier(gpInformation.gpSupplier)
                SerenityHelpers.setPatient(patient)
            }
            return patient
        }

        fun findOdsCode(gpSystem: Supplier?, journeyTypes: Collection<SJRJourneyType>): String {
            return journeysToGpInformationMap.find(gpSystem, journeyTypes).odsCode
        }

        private class ServiceJourneyRulesConfigurationBuilder {
            private val configs = mutableListOf<GpInformation>()

            fun add(supplier: Supplier, odsCode: String, journeys: EnumSet<SJRJourneyType>)
                    : ServiceJourneyRulesConfigurationBuilder {
                if (configs.any { config -> config.odsCode == odsCode }) {
                    Assert.fail("Duplicate ODS code in SJR setup: '$odsCode'")
                }
                configs.add(GpInformation(supplier, odsCode, journeys))
                return this
            }

            fun find(supplier: Supplier? = null, journeyTypes: Collection<SJRJourneyType>): GpInformation {
                val matchingConfig = configs.firstOrNull { config -> config.matches(supplier, journeyTypes) }
                Assert.assertNotNull(
                    "Test setup incorrect: Cannot find a matching ods code for system:" +
                            "'$supplier', with journeys $journeyTypes",
                    matchingConfig
                )
                return matchingConfig!!
            }

            data class GpInformation(
                val gpSupplier: Supplier, val odsCode: String,
                val journeyTypes: EnumSet<SJRJourneyType>
            ) {
                fun matches(targetSupplier: Supplier? = null, targetJourneyTypes: Collection<SJRJourneyType>): Boolean {
                    val matchesSupplier = targetSupplier == null || targetSupplier == gpSupplier
                    val matchesJourney = journeyTypes.size >= targetJourneyTypes.size &&
                            journeyTypes.containsAll(targetJourneyTypes)
                    return matchesSupplier && matchesJourney
                }
            }
        }
    }
}
