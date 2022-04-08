package features.wayfinder.factories

import features.authentication.stepDefinitions.AuthenticationFactoryVision.Companion.mockingClient
import mocking.wayfinder.WayfinderMappingBuilder

class WayfinderFactory {

   private val wayfinderMappingBuilder = WayfinderMappingBuilder()

    fun setupDelayedResponse() {
        mockingClient.forWayfinder.mock {
            wayfinderMappingBuilder.timeout()
        }
    }

    fun setupInternalServerError() {
        mockingClient.forWayfinder.mock {
            wayfinderMappingBuilder.internalServerError()
        }
    }

    fun setupNoReferralsOrAppointmentsResponse() {
        mockingClient.forWayfinder.mock {
            wayfinderMappingBuilder.noReferrals()
        }
    }

    fun setupReferralsResponse() {
        mockingClient.forWayfinder.mock {
            wayfinderMappingBuilder.referrals()
        }
    }
}
