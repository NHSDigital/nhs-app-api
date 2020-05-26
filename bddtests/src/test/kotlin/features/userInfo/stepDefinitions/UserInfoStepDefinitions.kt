package features.userInfo.stepDefinitions

import cucumber.api.java.en.Given
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mongodb.MongoDBConnection
import utils.SerenityHelpers
import utils.set

class UserInfoStepDefinitions {

    @Given("^I am a user who has user info enabled in SJR, and has not registered before$")
    fun iAmAUserWhoHasUserInfoEnabledInSJRAndHasNotRegisteredBefore() {
        createUser(SJRJourneyType.USER_INFO_ENABLED)
    }

    @Given("^I am a user who does not have user info enabled in SJR, and has not registered before$")
    fun iAmAUserWhoDoesNotHaveUserInfoEnabledInSJRAndHasNotRegisteredBefore() {
        createUser(SJRJourneyType.USER_INFO_DISABLED)
    }

    private fun createUser(journeyType: SJRJourneyType) {
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(null, journeyType)
        val supplier = SerenityHelpers.getGpSupplier()
        CitizenIdSessionCreateJourney().createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(supplier).createFor(patient)
        MongoDBConnection.UserInfoCollection.clearCache()
    }
}
