package features.serviceJourneyRules.factories

import constants.Supplier
import models.IdentityProofingLevel
import models.Patient
import org.junit.Assert
import utils.SerenityHelpers
import java.util.*

class ServiceJourneyRulesMapper {
    companion object {
        val ODSCODE_HOME_SCREEN_PUBLIC_HEALTH_NOTIFICATION = "A10006"

        private val journeysToGpInformationMap = ServiceJourneyRulesConfigurationBuilder()
                .add(Supplier.EMIS, "A11111",
                        EnumSet.of(SJRJourneyType.APPOINTMENTS_IM1,
                                SJRJourneyType.MEDICAL_RECORD_IM1,
                                SJRJourneyType.PRESCRIPTIONS_IM1,
                                SJRJourneyType.ONLINE_CONSULTATIONS_DISABLED,
                                SJRJourneyType.NOMINATED_PHARMACY_ENABLED,
                                SJRJourneyType.NOTIFICATIONS_DISABLED,
                                SJRJourneyType.MESSAGES_DISABLED,
                                SJRJourneyType.USER_INFO_DISABLED,
                                SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_PKB,
                                SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_PKB,
                                SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_PKB,
                                SJRJourneyType.SILVER_INTEGRATION_LIBRARY_PKB,
                                SJRJourneyType.SILVER_INTEGRATION_MESSAGES_PKB,
                                SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_PKB))
                .add(Supplier.EMIS, "A22222",
                        EnumSet.of(SJRJourneyType.APPOINTMENTS_INFORMATICA,
                                SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS,
                                SJRJourneyType.MEDICAL_RECORD_IM1,
                                SJRJourneyType.PRESCRIPTIONS_IM1,
                                SJRJourneyType.NOMINATED_PHARMACY_DISABLED))
                .add(Supplier.EMIS, "A44444",
                        EnumSet.of(SJRJourneyType.APPOINTMENTS_GPATHAND,
                                SJRJourneyType.MEDICAL_RECORD_GPATHAND,
                                SJRJourneyType.PRESCRIPTIONS_GPATHAND,
                                SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_NONE,
                                SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_NONE,
                                SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_NONE,
                                SJRJourneyType.SILVER_INTEGRATION_LIBRARY_NONE,
                                SJRJourneyType.SILVER_INTEGRATION_MESSAGES_NONE,
                                SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_NONE))
                .add(Supplier.TPP, "A55555",
                        EnumSet.of(SJRJourneyType.ONLINE_CONSULTATIONS_DISABLED,
                                SJRJourneyType.NOTIFICATIONS_ENABLED,
                                SJRJourneyType.MESSAGES_ENABLED,
                                SJRJourneyType.USER_INFO_ENABLED,
                                SJRJourneyType.IM1_MESSAGING_ENABLED,
                                SJRJourneyType.ONLINE_CONSULTATIONS_DISABLED))
                .add(Supplier.VISION, "A66666",
                        EnumSet.of(SJRJourneyType.ONLINE_CONSULTATIONS_DISABLED))
                .add(Supplier.EMIS, "A80001",
                        EnumSet.of(SJRJourneyType.MEDICAL_RECORD_VERSION_1,
                                SJRJourneyType.IM1_MESSAGING_ENABLED,
                                SJRJourneyType.IM1_MESSAGING_CANDELETEMESSAGES_DISABLED,
                                SJRJourneyType.IM1_MESSAGING_CANUPDATEREADSTATUS_ENABLED,
                                SJRJourneyType.IM1_MESSAGING_REQUIRESDETAILSREQUEST_ENABLED))
                .add(Supplier.VISION, "A80003",
                        EnumSet.of(SJRJourneyType.MEDICAL_RECORD_VERSION_1))
                .add(Supplier.MICROTEST, "A80004",
                        EnumSet.of(SJRJourneyType.MEDICAL_RECORD_VERSION_1))
                .add(Supplier.TPP, "A80002",
                        EnumSet.of(SJRJourneyType.MEDICAL_RECORD_VERSION_1,
                                SJRJourneyType.IM1_MESSAGING_ENABLED,
                                SJRJourneyType.IM1_MESSAGING_CANDELETEMESSAGES_DISABLED,
                                SJRJourneyType.IM1_MESSAGING_CANUPDATEREADSTATUS_ENABLED,
                                SJRJourneyType.IM1_MESSAGING_REQUIRESDETAILSREQUEST_DISABLED))
                .add(Supplier.EMIS, ODSCODE_HOME_SCREEN_PUBLIC_HEALTH_NOTIFICATION,
                        EnumSet.of(SJRJourneyType.HOME_SCREEN_PUBLIC_HEALTH_NOTIFICATIONS))
                .add(Supplier.EMIS, "A10003",
                        EnumSet.of(SJRJourneyType.DOCUMENTS_ENABLED,
                                SJRJourneyType.IM1_MESSAGING_ENABLED,
                                SJRJourneyType.IM1_MESSAGING_CANDELETEMESSAGES_DISABLED,
                                SJRJourneyType.IM1_MESSAGING_CANUPDATEREADSTATUS_DISABLED,
                                SJRJourneyType.IM1_MESSAGING_REQUIRESDETAILSREQUEST_DISABLED))
                .add(Supplier.EMIS, "A10004",
                        EnumSet.of(SJRJourneyType.DOCUMENTS_DISABLED,
                                SJRJourneyType.IM1_MESSAGING_DISABLED,
                                SJRJourneyType.IM1_MESSAGING_CANDELETEMESSAGES_DISABLED,
                                SJRJourneyType.IM1_MESSAGING_CANUPDATEREADSTATUS_DISABLED,
                                SJRJourneyType.IM1_MESSAGING_REQUIRESDETAILSREQUEST_DISABLED))
                .add(Supplier.EMIS, "A00002",
                        EnumSet.of(
                                SJRJourneyType.SILVER_INTEGRATION_CAREPLANS_PKB,
                                SJRJourneyType.SILVER_INTEGRATION_CONSULTATIONS_PKB,
                                SJRJourneyType.SILVER_INTEGRATION_HEALTHTRACKER_PKB,
                                SJRJourneyType.SILVER_INTEGRATION_LIBRARY_PKB,
                                SJRJourneyType.SILVER_INTEGRATION_MESSAGES_PKB_TESTSILVERTHIRDPARTYPROVIDER,
                                SJRJourneyType.SILVER_INTEGRATION_SECONDARY_APPOINTMENTS_ERS_PKB))

        fun findPatientForConfiguration(gpSystem: Supplier?,
                                        journeyType: SJRJourneyType,
                                        proofLevel: IdentityProofingLevel? = null): Patient {
            return findPatientForConfiguration(gpSystem, arrayListOf(journeyType), proofLevel)
        }

        fun findPatientForConfiguration(gpSystem: Supplier?,
                                        journeyTypes: Collection<SJRJourneyType>,
                                        proofLevel: IdentityProofingLevel? = null): Patient {
            val gpInformation = journeysToGpInformationMap.find(gpSystem, journeyTypes)
            var patient = Patient.getDefault(gpInformation.gpSupplier).copy(odsCode = gpInformation.odsCode)
            if (proofLevel != null) {
                patient = patient.copy(identityProofingLevel = proofLevel)
            }
            SerenityHelpers.setGpSupplier(gpInformation.gpSupplier)
            SerenityHelpers.setPatient(patient)
            return patient
        }

        fun findOdsCode(gpSystem: Supplier?, journeyTypes: ArrayList<SJRJourneyType>): String {
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
                Assert.assertNotNull("Test setup incorrect: Cannot find a matching ods code for system:" +
                        "'$supplier', with journeys $journeyTypes",
                        matchingConfig)
                return matchingConfig!!
            }

            internal data class GpInformation(val gpSupplier: Supplier, val odsCode: String,
                                              val journeyTypes: EnumSet<SJRJourneyType>) {
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
