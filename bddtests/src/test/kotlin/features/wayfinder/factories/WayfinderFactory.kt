package features.wayfinder.factories

import features.authentication.stepDefinitions.AuthenticationFactoryVision.Companion.mockingClient
import mocking.apim.ApimMappingBuilder
import mocking.wayfinder.WayfinderMappingBuilder

class WayfinderFactory {

    private val wayfinderMappingBuilder = WayfinderMappingBuilder()
    private val apimMappingBuilder = ApimMappingBuilder()

    fun setupDelayedResponse() {
        mockingClient.forWayfinder.mock {
            apimMappingBuilder.successfulTokenRequest()
        }

        mockingClient.forWayfinder.mock {
            wayfinderMappingBuilder.timeout()
        }
    }

    fun setupInternalServerError() {
        mockingClient.forWayfinder.mock {
            apimMappingBuilder.successfulTokenRequest()
        }

        mockingClient.forWayfinder.mock {
            wayfinderMappingBuilder.internalServerError()
        }
    }

    fun setupNoReferralsOrAppointmentsResponse() {
        mockingClient.forWayfinder.mock {
            apimMappingBuilder.successfulTokenRequest()
        }

        mockingClient.forWayfinder.mock {
            wayfinderMappingBuilder.noReferralsOrUpcomingAppointments()
        }
    }

    fun setupReferralsAndUpcomingAppointmentsResponse() {
        mockingClient.forWayfinder.mock {
            apimMappingBuilder.successfulTokenRequest()
        }

        mockingClient.forWayfinder.mock {
            wayfinderMappingBuilder.referralsAndUpcomingAppointments()
        }
    }

    fun setupReferralsNoAppointmentsResponse() {
        mockingClient.forWayfinder.mock {
            apimMappingBuilder.successfulTokenRequest()
        }

        mockingClient.forWayfinder.mock {
            wayfinderMappingBuilder.referralsNoAppointments()
        }
    }

    fun setupReferralsAppointmentsPartialErrorResponse() {
        mockingClient.forWayfinder.mock {
            apimMappingBuilder.successfulTokenRequest()
        }

        mockingClient.forWayfinder.mock {
            wayfinderMappingBuilder.referralsAppointmentsPartialError()
        }
    }
}
