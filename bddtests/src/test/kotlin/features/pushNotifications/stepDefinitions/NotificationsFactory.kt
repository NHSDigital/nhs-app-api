package features.pushNotifications.stepDefinitions

import features.serviceJourneyRules.factories.ServiceJourneyRulesConfiguration
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.AccessTokenBuilder
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import mongodb.MongoDBConnection
import mongodb.MongoRepositoryUserDevice
import org.junit.Assert
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.addToList
import utils.getOrFail
import utils.set
import java.util.*

class NotificationsFactory {

    val mockingClient = MockingClient.instance

    fun setUpUser(gpSystem: String? = null, patient: Patient? = null): Patient {
        val patientToUse = patient ?: ServiceJourneyRulesMapper.findPatientForConfiguration(gpSystem,
                listOf(ServiceJourneyRulesConfiguration("notifications", "enabled")))

        val gpSystemToUse = gpSystem ?: SerenityHelpers.getGpSupplier()
        SerenityHelpers.setPatient(patientToUse)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patientToUse)
        SessionCreateJourneyFactory.getForSupplier(gpSystemToUse, mockingClient).createFor(patientToUse)
        MongoDBConnection.UserDevicesCollection.clearCache()
        return patientToUse
    }

    fun setUpAlternativeUser(): Patient {
        // Use SJR generated patient, but then change subject and access token based on that,
        // to create a new nhsLoginId and differentiate from the primary patient
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(SerenityHelpers.getGpSupplier(),
                listOf(ServiceJourneyRulesConfiguration("notifications", "enabled")))
        patient.subject = UUID.randomUUID().toString()
        patient.accessToken = AccessTokenBuilder().getSignedToken(patient).serialize()
        CitizenIdSessionCreateJourney(mockingClient)
                .createFor(patient, alternativeUser = true)
        SessionCreateJourneyFactory.getForSupplier(SerenityHelpers.getGpSupplier(), mockingClient)
                .createFor(patient, alternativeUser = true)
        MongoDBConnection.UserDevicesCollection.clearCache()
        return patient
    }

    fun setUpDeviceValues() {
        val deviceType = "Android"
        val devicePns = "0123456789ABCDEF"
        PushNotificationsSerenityHelpers.EXPECTED_DEVICE_TYPE.set(deviceType)
        PushNotificationsSerenityHelpers.EXPECTED_PNS.set(devicePns)
    }

    fun mockNativeNotificationFunctions() {
        val pns = PushNotificationsSerenityHelpers.EXPECTED_PNS.getOrFail<String>()
        val deviceType = PushNotificationsSerenityHelpers.EXPECTED_DEVICE_TYPE.getOrFail<String>().toLowerCase()
        val notificationsAuthorisedFunction = mockNotificationsAuthorised(pns, deviceType)
        GlobalSerenityHelpers.FUNCTIONS_TO_ADD_TO_WINDOW_NATIVE_APP_OBJECT.addToList(notificationsAuthorisedFunction)
        val notificationsEnabledFunction = mockNotificationsEnabled()
        GlobalSerenityHelpers.FUNCTIONS_TO_ADD_TO_WINDOW_NATIVE_APP_OBJECT.addToList(notificationsEnabledFunction)
    }

    fun setUpExistingRegistration(patient: Patient? = null) {
        val patientToUse = patient ?: SerenityHelpers.getPatient()
        val authToken = patientToUse.accessToken
        NotificationsApi.postRegistration(authToken)
        val userDevices = MongoDBConnection.UserDevicesCollection
                .getValues<MongoRepositoryUserDevice>(MongoRepositoryUserDevice::class.java)
        Assert.assertNotNull(userDevices)
    }

    private fun mockNotificationsEnabled(): String {
        return "areNotificationsEnabled : " +
                "function(){window.\$nuxt.\$store.dispatch(" +
                "'notifications/settingsEnabled', true)}"
    }

    private fun mockNotificationsAuthorised(pns: String, deviceType: String):String {
        return "requestPnsToken : " +
                "function(trigger){window.\$nuxt.\$store.dispatch(" +
                "'notifications/authorised', " +
                "'{\"devicePns\":\"$pns\",\"deviceType\":\"$deviceType\", \"trigger\":\"' + trigger +'\"}')}"
    }
}