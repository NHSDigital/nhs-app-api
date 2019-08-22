package features.pushNotifications.stepDefinitions

import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import mongodb.MongoDBConnection
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.addToList
import utils.getOrFail
import utils.set
import worker.models.userDevices.RegisterUserDevicesRequest

class NotificationsFactory{

    val mockingClient = MockingClient.instance

    fun setUpUser(gpSystem: String, patient:Patient? = null) {
        SerenityHelpers.setGpSupplier(gpSystem)
        val patientToUse = patient?: Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patientToUse)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patientToUse)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patientToUse)
        MongoDBConnection.UserDevicesCollection.clearCache()
    }

    fun setUpRegistration() {
        val deviceType = "Android"
        val devicePns = "0123456789ABCDEF"
        PushNotificationsSerenityHelpers.EXPECTED_DEVICE_TYPE.set(deviceType)
        PushNotificationsSerenityHelpers.EXPECTED_PNS.set(devicePns)
        PushNotificationsSerenityHelpers.REGISTER_REQUEST.set(RegisterUserDevicesRequest(devicePns, deviceType))
    }

    fun mockPNS(authorised:Boolean) {
        val pns = PushNotificationsSerenityHelpers.EXPECTED_PNS.getOrFail<String>()
        val deviceType =  PushNotificationsSerenityHelpers.EXPECTED_DEVICE_TYPE.getOrFail<String>().toLowerCase()
        val unauthorisedResponse = "window.\$nuxt.\$store.dispatch('notifications/unAuthorised');"
        val authorisedResponse = "window.\$nuxt.\$store.dispatch(" +
                "'notifications/authorised', " +
                "'{\"devicePns\":\"$pns\",\"deviceType\":\"$deviceType\"}');"
        val response = if (authorised) authorisedResponse else unauthorisedResponse
        val function = "window.nativeApp = { requestPnsToken : function(){$response}}"
        GlobalSerenityHelpers.JAVASCRIPT_TO_EXECUTE_ON_WINDOW.addToList(function)
    }
}