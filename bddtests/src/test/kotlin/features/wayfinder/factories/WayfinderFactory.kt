package features.wayfinder.factories

import features.authentication.stepDefinitions.AuthenticationFactoryVision.Companion.mockingClient
import mocking.wayfinder.WayfinderMappingBuilder

class WayfinderFactory {

   private val wayfinderMappingBuilder = WayfinderMappingBuilder()

    fun setupNoReferralsOrAppointmentsResponse() {
        mockingClient.forWayfinder.mock {
            wayfinderMappingBuilder.noReferrals();
        }
    }

    fun setupReferralsResponse() {
        mockingClient.forWayfinder.mock {
            wayfinderMappingBuilder.referrals();
        }
    }
}
