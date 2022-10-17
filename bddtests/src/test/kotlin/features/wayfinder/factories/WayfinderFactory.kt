package features.wayfinder.factories

import mocking.apim.ApimMappingBuilder
import mocking.wayfinder.WayfinderMappingBuilder
import utils.SerenityHelpers

class WayfinderFactory {

    private val wayfinderMappingBuilder = WayfinderMappingBuilder()
    private val apimMappingBuilder = ApimMappingBuilder()
    private val mockingClient = SerenityHelpers.getMockingClient()

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

    fun setupReferralsAppointmentsUnderMinimumAgeResponse() {
        mockingClient.forWayfinder.mock {
            apimMappingBuilder.successfulTokenRequest()
        }

        mockingClient.forWayfinder.mock {
            wayfinderMappingBuilder.referralsAppointmentsUnderMinimumAgeError()
        }
    }

    fun setupReferralsAndAppointments(provider: String){
        mockingClient.forWayfinder.mock {
            apimMappingBuilder.successfulTokenRequest()
        }

        mockingClient.forWayfinder.mock {
            wayfinderMappingBuilder.referralsAndUpcomingAppointments(provider)
        }
    }

    fun setupWaitTimes() {
        mockingClient.forWayfinder.mock {
            apimMappingBuilder.successfulTokenRequest()
        }

        mockingClient.forWayfinder.mock {
            wayfinderMappingBuilder.waitTimes()
        }
    }

    fun setupWaitTimesError() {
        mockingClient.forWayfinder.mock {
            apimMappingBuilder.successfulTokenRequest()
        }

        mockingClient.forWayfinder.mock {
            wayfinderMappingBuilder.waitTimesError()
        }
    }
}
