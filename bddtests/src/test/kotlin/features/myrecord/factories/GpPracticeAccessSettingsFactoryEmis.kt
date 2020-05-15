package features.myrecord.factories

import mocking.emis.models.MeSettingsResponseModel
import mocking.emis.models.UserGpPracticeSettingsResponseModel
import mockingFacade.linkedProfiles.FeaturesEnabledFacade
import models.Patient

class GpPracticeAccessSettingsFactoryEmis: GpPracticeAccessSettingsFactory() {
    override fun enabled(patient: Patient) {
        mockingClient.forEmis.mock {
            authentication.meSettingsRequest(patient = patient)
                    .respondWithSuccess(
                            MeSettingsResponseModel(
                                    UserGpPracticeSettingsResponseModel(
                                            appointmentsEnabled = true,
                                            prescribingEnabled = false,
                                            medicalRecordEnabled = true)
                            )
                    )
        }
    }

    override fun enabledViaProxy(
            callingPatient: Patient,
            actingOnBehalfOf: Patient,
            featuresEnabled: FeaturesEnabledFacade) {
        mockingClient.forEmis.mock {
            authentication.meSettingsRequest(
                    patient = actingOnBehalfOf,
                    sessionId = callingPatient.sessionId,
                    endUserSessionId = callingPatient.endUserSessionId)
                    .respondWithSuccess(
                            MeSettingsResponseModel(
                                    UserGpPracticeSettingsResponseModel(
                                            appointmentsEnabled = featuresEnabled.appointmentsEnabled,
                                            prescribingEnabled = featuresEnabled.prescribingEnabled,
                                            medicalRecordEnabled = featuresEnabled.medicalRecordEnabled)
                            )
                    )
        }
    }
}
