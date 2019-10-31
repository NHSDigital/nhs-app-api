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
import worker.models.userDevices.InvalidUserDevice
import java.util.*

class NotificationsFactory {

    val mockingClient = MockingClient.instance
    private val devicePns = "0123456789ABCDEF"

    fun setUpUser(gpSystem: String? = null, patient: Patient? = null): Patient {
        val patientToUse = patient ?: ServiceJourneyRulesMapper.findPatientForConfiguration(gpSystem,
                listOf(ServiceJourneyRulesConfiguration("notifications", "enabled")))

        val gpSystemToUse = gpSystem ?: SerenityHelpers.getGpSupplier()
        SerenityHelpers.setPatient(patientToUse)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patientToUse)
        SessionCreateJourneyFactory.getForSupplier(gpSystemToUse, mockingClient).createFor(patientToUse)
        MongoDBConnection.UserDevicesCollection.clearCache()
        PushNotificationsSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(patientToUse.subject)
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
        PushNotificationsSerenityHelpers.EXPECTED_DEVICE_TYPE.set(deviceType)
        PushNotificationsSerenityHelpers.EXPECTED_PNS.set(devicePns)
    }

    fun mockNativeNotificationFunctions(status: SettingStatus, authorised: Boolean = true) {
        val pns = PushNotificationsSerenityHelpers.EXPECTED_PNS.getOrFail<String>()
        val deviceType = PushNotificationsSerenityHelpers.EXPECTED_DEVICE_TYPE.getOrFail<String>().toLowerCase()
        val notificationsFunction =
                if (authorised) mockNotificationsAuthorised(pns, deviceType)
                else mockNotificationsUnauthorised()
        GlobalSerenityHelpers.FUNCTIONS_TO_ADD_TO_WINDOW_NATIVE_APP_OBJECT.addToList(notificationsFunction)
        val notificationsStatusFunction = mockNotificationsStatus(status)
        GlobalSerenityHelpers.FUNCTIONS_TO_ADD_TO_WINDOW_NATIVE_APP_OBJECT.addToList(notificationsStatusFunction)
    }

    fun setUpExistingRegistration(patient: Patient? = null) {
        val patientToUse = patient ?: SerenityHelpers.getPatient()
        val authToken = patientToUse.accessToken
        NotificationsApi.postRegistration(authToken)
        val userDevices = MongoDBConnection.UserDevicesCollection
                .getValues<MongoRepositoryUserDevice>(MongoRepositoryUserDevice::class.java)
        Assert.assertNotNull(userDevices)
    }

    fun setUpInvalidMongoDeviceRegistration() {
        MongoDBConnection.UserDevicesCollection.clearCache()
        val patient = SerenityHelpers.getPatient()
        val device = createInvalidDevice(patient)
        createUserDeviceInRepository(device)
    }

    private fun mockNotificationsStatus(status: SettingStatus): String {
        return "getNotificationsStatus : " +
                "function(){window.\$nuxt.\$store.dispatch(" +
                "'notifications/settingsStatus', '${status.name.decapitalize()}')}"
    }

    private fun mockNotificationsAuthorised(pns: String, deviceType: String): String {
        return "requestPnsToken : " +
                "function(trigger){window.\$nuxt.\$store.dispatch(" +
                "'notifications/authorised', " +
                "'{\"devicePns\":\"$pns\",\"deviceType\":\"$deviceType\", \"trigger\":\"' + trigger +'\"}')}"
    }

    private fun mockNotificationsUnauthorised(): String {
        return "requestPnsToken : " +
                "function(trigger){window.\$nuxt.\$store.dispatch(" +
                "'notifications/unauthorised')}"
    }

    private fun createInvalidDevice(patient: Patient): InvalidUserDevice {
        return InvalidUserDevice(
                patient.subject + "-" + devicePns,
                patient.subject,
                "this is an invalid field")
    }

    private fun createUserDeviceInRepository(userDevice: InvalidUserDevice) {
        MongoDBConnection.UserDevicesCollection.clearAndInsertValue(userDevice)
    }
}
