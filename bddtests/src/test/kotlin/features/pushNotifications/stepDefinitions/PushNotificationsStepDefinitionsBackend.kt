package features.pushNotifications.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.sharedSteps.InvalidAccessTokenTester
import mongodb.MongoDBConnection
import mongodb.MongoRepositoryUserDevice
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import worker.models.userDevices.RegisterUserDevicesResponse

class PushNotificationsStepDefinitionsBackend {

    @Given("^I am an api user wishing to register their device for push notifications$")
    fun iAmAnApiUserWishingToRegisterTheirDeviceForPushNotifications() {
        val factory = NotificationsFactory()
        factory.setUpUser()
        factory.setUpDeviceValues()
    }

    @Given("^I am an api user who has registered their device for notifications," +
            " and I have another device to register$")
    fun iAmAnApiUserWhoHasRegisteredTheirDeviceForNotificationsAndIHaveAnotherDeviceToRegister() {
        val factory = NotificationsFactory()
        val patient = factory.setUpUser()
        factory.setUpDeviceValues()
        factory.setUpExistingRegistration()

        val originalPnsToken = PushNotificationsSerenityHelpers.EXPECTED_PNS.getOrFail<String>()
        val newPnsToRegister =  PnsTokenGenerator.generate()
        PushNotificationsSerenityHelpers.EXPECTED_PNS.set(newPnsToRegister)
        PushNotificationsSerenityHelpers.EXPECTED_MULTIPLE_PNS.set(arrayListOf(originalPnsToken, newPnsToRegister))
        factory.setUpDeletionAfterTest(newPnsToRegister, patient.accessToken)
    }

    @Given("^I am an api user who has not registered their device for push notifications$")
    fun iAmAnApiUserWhoHasNotRegisteredTheirDeviceForPushNotifications() {
        val factory = NotificationsFactory()
        factory.setUpUser()
        factory.setUpDeviceValues()
    }

    @Given("^I am an api user who has registered their device for push notifications$")
    fun iAmAnApiUserWhoHasRegisteredTheirDeviceForPushNotifications() {
        val factory = NotificationsFactory()
        factory.setUpUser()
        factory.setUpDeviceValues()
        factory.setUpExistingRegistration()
    }

    @Given("^I am an api user whose notification registration in azure has been overridden by another user$")
    fun iAmAnApiUserWhoseRegistrationInAzureHasBeenOverriddenByAnotherUser() {
        val factory = NotificationsFactory()
        factory.setUpDeviceValues()
        val user1 = factory.setUpUser()
        val user2 = factory.setUpAlternativeUser()
        factory.setUpDeletionAfterTest(PushNotificationsSerenityHelpers.EXPECTED_PNS.getOrFail(), user2.accessToken)
        factory.setUpExistingRegistration(user1)
        factory.setUpExistingRegistration(user2)
    }

    @Given("^I am an api user wishing to delete their registration for push notifications$")
    fun iAmAnApiUserWishingToDeleteTheirRegistrationForPushNotifications() {
        val factory = NotificationsFactory()
        factory.setUpUser()
        factory.setUpDeviceValues()
        factory.setUpExistingRegistration()
    }

    @When("^I register the device for push notifications$")
    fun iRegisterTheDeviceForPushNotifications() {
        val authToken = SerenityHelpers.getPatient().accessToken
        NotificationsApi.postRegistration(authToken = authToken)
    }

    @When("^I get the registration for push notifications$")
    fun iGetTheRegistrationForPushNotifications() {
        val authToken = SerenityHelpers.getPatient().accessToken
        NotificationsApi.getRegistration(authToken = authToken)
    }

    @When("^I get the registration for push notifications with an unregistered device pns token$")
    fun iGetTheRegistrationForPushNotificationsWithAnUnregisteredDeviceId() {
        val unregisteredDevicePns =  PnsTokenGenerator.generate()
        PushNotificationsSerenityHelpers.EXPECTED_PNS.set(unregisteredDevicePns)
        val authToken = SerenityHelpers.getPatient().accessToken
        NotificationsApi.getRegistration(authToken = authToken)
    }

    @When("^I delete the registration for push notifications$")
    fun iDeleteTheRegistrationForPushNotifications() {
        val authToken = SerenityHelpers.getPatient().accessToken
        NotificationsApi.deleteRegistration(authToken = authToken)
    }

    @When("^I delete the registration for push notifications with an unregistered device pns token$")
    fun iDeleteTheRegistrationForPushNotificationsWithAnUnregisteredDeviceId() {
        val unregisteredDevicePns =  PnsTokenGenerator.generate()
        PushNotificationsSerenityHelpers.EXPECTED_PNS.set(unregisteredDevicePns)
        val authToken = SerenityHelpers.getPatient().accessToken
        NotificationsApi.deleteRegistration(authToken = authToken)
    }

    @When("^I delete the registration for push notifications without a pns token$")
    fun iDeleteTheRegistrationForPushNotificationsWithoutAPnsToken() {
        PushNotificationsSerenityHelpers.EXPECTED_PNS.set("")
        val authToken = SerenityHelpers.getPatient().accessToken
        NotificationsApi.deleteRegistration(authToken = authToken)
    }

    @When("^I get the registration for push notifications without a pns token$")
    fun iGetTheRegistrationForPushNotificationsWithoutAPnsToken() {
        PushNotificationsSerenityHelpers.EXPECTED_PNS.set("")
        val authToken = SerenityHelpers.getPatient().accessToken
        NotificationsApi.getRegistration(authToken = authToken)
    }

    @When("^I delete the registration for push notifications without auth token$")
    fun iDeleteTheRegistrationForPushNotificationsWithoutAuthToken() {
        NotificationsApi.deleteRegistration(authToken = null)
    }

    @When("^I get the registration for push notifications without auth token$")
    fun iGetTheRegistrationForPushNotificationsWithoutAuthToken() {
        NotificationsApi.getRegistration(authToken = null)
    }

    @When("^I register the device for push notifications without auth token$")
    fun iRegisterTheDeviceForPushNotificationsWithoutAuthToken() {
        NotificationsApi.postRegistration(authToken = null)
    }

    @When("^I register the device for push notifications without a pns token$")
    fun iRegisterTheDeviceForPushNotificationsWithoutAPnsToken() {
        PushNotificationsSerenityHelpers.EXPECTED_PNS.set("")
        val authToken = SerenityHelpers.getPatient().accessToken
        NotificationsApi.postRegistration(authToken = authToken)
    }

    @When("^a registration attempt with an invalid access token will return an Unauthorised error$")
    fun aRegistrationAttemptWithAnInvalidAccessTokenWillReturnAnUnauthorisedError() {
        InvalidAccessTokenTester.assertInvalidTokensThrowUnauthorised { accessToken ->
            NotificationsApi.postRegistration(authToken = accessToken)
        }
    }

    @When("^getting a notification registration with an invalid access token will return an Unauthorised error$")
    fun anAttemptToGetTheNotificationRegistrationWithAnInvalidAccessTokenWillReturnAnUnauthorisedError() {
        InvalidAccessTokenTester.assertInvalidTokensThrowUnauthorised { accessToken ->
            NotificationsApi.getRegistration(authToken = accessToken)
        }
    }

    @When("^deleting a notification registration with an invalid access token will return an Unauthorised error$")
    fun anAttemptToDeleteTheNotificationRegistrationWithAnInvalidAccessTokenWillReturnAnUnauthorisedError() {
        InvalidAccessTokenTester.assertInvalidTokensThrowUnauthorised { accessToken ->
            NotificationsApi.deleteRegistration(authToken = accessToken)
        }
    }

    @Then("^I receive the newly created registered device details$")
    fun iReceiveTheNewlyCreatedRegisteredDeviceDetails() {
        val response =
                PushNotificationsSerenityHelpers.REGISTER_RESPONSE.getOrFail<RegisterUserDevicesResponse>()
        val expectedDeviceType =
                PushNotificationsSerenityHelpers.EXPECTED_DEVICE_TYPE.getOrFail<String>()

        Assert.assertNotNull("Register User Devices response", response)
        Assert.assertNotNull("Register User Devices response DeviceId", response.deviceId)
        Assert.assertEquals("Register User Devices response DeviceType",
                expectedDeviceType,
                response.deviceType)
    }

    @Then("^the device registration is not available in the device repository$")
    fun theDeviceRegistrationIsNotAvailableInTheDeviceRepository() {
        //Cache has been cleared in data setup.
        MongoDBConnection.UserDevicesCollection.assertNumberOfDocuments(0)
    }

    @Then("^my registration is not in the device repository, but the superseding registration exists$")
    fun myRegistrationIsNotInTheDeviceRepositoryButTheSupersedingRegistrationsExists() {
        //Cache has been cleared in data setup.
        assertSingleRecordInDeviceRepository()
    }

    @Then("^the device registration is available in the device repository$")
    fun theDeviceRegistrationIsAvailableInTheDeviceRepository() {
        //Cache has been cleared in data setup.
        assertSingleRecordInDeviceRepository()
    }

    @Then("^both device registrations are available in the device repository$")
    fun bothDeviceRegistrationsAreAvailableInTheDeviceRepository() {
        //Cache has been cleared in data setup.
        val userDevices = assertNumberOfDevicesInRepository(2)
        userDevices.forEach { record -> assertFoundDeviceRecordNotNull(record) }
        val registeredPnsTokens = userDevices.map { record -> record.PnsToken }
        val expectedPns = PushNotificationsSerenityHelpers.EXPECTED_MULTIPLE_PNS.getOrFail<ArrayList<String>>()
        Assert.assertArrayEquals("Registered device PNS Tokens",
                expectedPns.toTypedArray(),
                registeredPnsTokens.toTypedArray())
    }

    private fun assertSingleRecordInDeviceRepository() {
        val userDevices = assertNumberOfDevicesInRepository(1)
        val registeredDevice = userDevices.single()
        assertFoundDeviceRecordNotNull(registeredDevice)
        val expectedPns = PushNotificationsSerenityHelpers.EXPECTED_PNS.getOrFail<String>()
        Assert.assertEquals("Registered device pnsToken", expectedPns, registeredDevice.PnsToken)
    }

    private fun assertFoundDeviceRecordNotNull(registeredDevice: MongoRepositoryUserDevice) {
        Assert.assertNotNull("Registered device id. $registeredDevice", registeredDevice._id)
        Assert.assertNotNull("Registered device nhsLoginId. $registeredDevice", registeredDevice.NhsLoginId)
        Assert.assertNotNull("Registered device registrationId. $registeredDevice", registeredDevice.RegistrationId)
        Assert.assertNotNull("Registered device pnsToken. $registeredDevice", registeredDevice.PnsToken)
    }

    private fun assertNumberOfDevicesInRepository(expectedNumber: Int): List<MongoRepositoryUserDevice> {
        MongoDBConnection.UserDevicesCollection.assertNumberOfDocuments(expectedNumber)
        val userDevices = MongoDBConnection.UserDevicesCollection
                .getValues<MongoRepositoryUserDevice>(MongoRepositoryUserDevice::class.java)
        Assert.assertNotNull("User devices", userDevices)
        Assert.assertEquals("Number of user devices", expectedNumber, userDevices.count())
        return userDevices
    }
}
