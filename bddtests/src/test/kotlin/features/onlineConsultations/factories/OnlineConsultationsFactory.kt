package features.onlineConsultations.factories

import features.authentication.stepDefinitions.AuthenticationFactoryVision.Companion.mockingClient
import mocking.onlineConsultations.OnlineConsultationsMappingBuilder

class OnlineConsultationsFactory {

   private val onlineConsultationsMappingBuilder = OnlineConsultationsMappingBuilder()

    fun setupOnlineConsultationsData() {
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.isValidRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.termsAndConditionsRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.conditionsRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.selfOrChildRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.genderRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.urgencyQuestion().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.emergencyEndOfConsultationConfiguration().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.emergencyQuestion().respondWithSuccess()
        }
    }

    fun setupOnlineConsultationsDataNonEmergency() {
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.isValidRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.termsAndConditionsRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.conditionsRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.selfOrChildRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.urgencyQuestion().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.genderRequest().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.howWeCanHelpQuestion().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.dobQuestion().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.alcoholQuestion().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.imageQuestion().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.quantityQuestion().respondWithSuccess()
        }
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.carePlan().respondWithSuccess()
        }
    }

    fun setupOnlineConsultationsDataIsNotValid() {
        mockingClient.forOnlineConsultations {
            onlineConsultationsMappingBuilder.isValidRequest(false).respondWithSuccess()
        }
    }
}