package features.pushNotifications.stepDefinitions

import constants.Supplier
import features.serviceJourneyRules.factories.SJRJourneyType
import features.serviceJourneyRules.factories.ServiceJourneyRulesMapper
import mocking.AccessTokenBuilder
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import mongodb.MongoDBConnection
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.addToList
import utils.set
import utils.getOrFail
import worker.models.userDevices.InvalidUserDevice
import java.util.*

class NotificationsFactory {

    fun setUpUser(supplier: Supplier? = null, patient: Patient? = null): Patient {
        val patientToUse = patient ?: ServiceJourneyRulesMapper.findPatientForConfiguration(supplier,
                SJRJourneyType.NOTIFICATIONS_ENABLED)
        val supplierToUse = supplier ?: SerenityHelpers.getGpSupplier()
        SerenityHelpers.setPatient(patientToUse)
        CitizenIdSessionCreateJourney().createFor(patientToUse)
        SessionCreateJourneyFactory.getForSupplier(supplierToUse).createFor(patientToUse)
        MongoDBConnection.UserDevicesCollection.clearCache()
        PushNotificationsSerenityHelpers.EXPECTED_NHS_LOGIN_ID.set(patientToUse.subject)
        return patientToUse
    }

    fun setUpAlternativeUser(): Patient {
        // Use SJR generated patient, but then change subject and access token based on that,
        // to create a new nhsLoginId and differentiate from the primary patient
        val patient = ServiceJourneyRulesMapper.findPatientForConfiguration(SerenityHelpers.getGpSupplier(),
                SJRJourneyType.NOTIFICATIONS_ENABLED)
        patient.subject = UUID.randomUUID().toString()
        patient.accessToken = AccessTokenBuilder().getSignedToken(patient).serialize()
        CitizenIdSessionCreateJourney()
                .createFor(patient, alternativeUser = true)
        SessionCreateJourneyFactory.getForSupplier(SerenityHelpers.getGpSupplier())
                .createFor(patient, alternativeUser = true)
        MongoDBConnection.UserDevicesCollection.clearCache()
        return patient
    }

    fun setUpDeviceValues() {
        val devicePns = PnsTokenGenerator.generate()
        val deviceType = "Android"
        PushNotificationsSerenityHelpers.EXPECTED_DEVICE_TYPE.set(deviceType)
        PushNotificationsSerenityHelpers.EXPECTED_PNS.set(devicePns)
        setUpDeletionAfterTest(devicePns)
    }

    fun setUpDeletionAfterTest(pnsToken:String, accessToken :String? = null) {
        val deletion = {
            NotificationsApi.deleteRegistration(
                    accessToken ?: SerenityHelpers.getPatient().accessToken,
                    pnsToken) }
        GlobalSerenityHelpers.TEAR_DOWN_ACTIONS.addToList(deletion)
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
        NotificationsApi.setupRegistration(authToken)
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
        return """requestPnsToken : function(trigger){
                    window.${'$'}nuxt.${'$'}store.dispatch(
                        'notifications/authorised',
                        {
                            devicePns:'$pns',
                            deviceType:'$deviceType', 
                            trigger
                        }
                    )
                }"""
    }

    private fun mockNotificationsUnauthorised(): String {
        return "requestPnsToken : " +
                "function(trigger){window.\$nuxt.\$store.dispatch(" +
                "'notifications/unauthorised')}"
    }

    private fun createInvalidDevice(patient: Patient): InvalidUserDevice {
        val devicePns =  PushNotificationsSerenityHelpers.EXPECTED_PNS.getOrFail<String>()
        return InvalidUserDevice(
                patient.subject + "-" + devicePns,
                patient.subject,
                "this is an invalid field")
    }

    private fun createUserDeviceInRepository(userDevice: InvalidUserDevice) {
        MongoDBConnection.UserDevicesCollection.clearAndInsertValue(userDevice)
    }
}
