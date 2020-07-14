package features.onlineConsultations.factories

import features.authentication.stepDefinitions.AuthenticationFactoryVision.Companion.mockingClient
import mocking.onlineConsultations.OnlineConsultationsMappingBuilder

class OnlineConsultationsFactory {

   private val onlineConsultationsMappingBuilder = OnlineConsultationsMappingBuilder()

    fun setupOnlineConsultationsData() {
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.isValidRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.termsAndConditionsRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.conditionsRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.selfOrChildRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.genderRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.urgencyQuestion().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.emergencyEndOfConsultationConfiguration().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.emergencyQuestion().respondWithSuccess()
        }
    }

    fun setupOnlineConsultationsDataNonEmergency(hasGpSession: Boolean = true) {
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.isValidRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.termsAndConditionsRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.conditionsRequest(hasGpSession).respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.selfOrChildRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.urgencyQuestion().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.genderRequest(hasGpSession).respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.howWeCanHelpQuestion().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.dobQuestion().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.alcoholQuestion().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.imageQuestion().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.quantityQuestion().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.carePlan().respondWithSuccess()
        }
    }

    fun setupOnlineConsultationsDataIsNotValid() {
        mockingClient.forOnlineConsultations.mock {
            onlineConsultationsMappingBuilder.isValidRequest(false).respondWithSuccess()
        }
    }
}
