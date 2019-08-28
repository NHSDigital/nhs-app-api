package features.pushNotifications.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.AccessTokenBuilder
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import mongodb.MongoDBConnection
import mongodb.MongoRepositoryUserDevice
import net.serenitybdd.core.Serenity.sessionVariableCalled
import org.apache.http.HttpStatus
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import utils.set
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.userDevices.RegisterUserDevicesRequest
import worker.models.userDevices.RegisterUserDevicesResponse

class PushNotificationsStepDefinitionsBackend {

    val mockingClient = MockingClient.instance

    @Given("^I am a (\\w+) api user wishing to register their device for push notifications$")
    fun iAmAApiUserWishingToRegisterTheirDeviceForPushNotifications(gpSystem: String) {
        setUpUser(gpSystem)
        MongoDBConnection.UserDevicesCollection.clearCache()
        setUpRegistration()
    }

    private fun setUpUser(gpSystem: String) {
        SerenityHelpers.setGpSupplier(gpSystem)
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)
    }

    private fun setUpRegistration() {
        val deviceType = "Android"
        val devicePns = "devicePns1234"
        PushNotificationsSerenityHelpers.EXPECTED_DEVICE_TYPE.set(deviceType)
        PushNotificationsSerenityHelpers.EXPECTED_PNS.set(devicePns)
        PushNotificationsSerenityHelpers.REGISTER_REQUEST.set(RegisterUserDevicesRequest(devicePns, deviceType))
    }

    @When("^I register the device for push notifications$")
    fun iRegisterTheDeviceForPushNotifications() {
        val authToken = Patient.getAccessToken(SerenityHelpers.getPatient())
        registerTheDeviceForPushNotifications(authToken = authToken)
    }

    @When("^I register the device for push notifications without auth token$")
    fun iRegisterTheDeviceForPushNotificationsWithoutAuthToken() {
        registerTheDeviceForPushNotifications(authToken = null)
    }

    @When("^a registration attempt with an invalid access token will return an Unauthorised error$")
    fun aRegistrationAttemptWithAnInvalidAccessTokenWillReturnAnUnauthorisedError() {
        val invalidTokens = AccessTokenBuilder().getInvalidTokens(SerenityHelpers.getPatient())

        invalidTokens.forEach { invalidToken ->
            testInvalidToken(accessToken = invalidToken.first.serialize(), tokenParameterKey = invalidToken.second)
        }
    }

    private fun testInvalidToken(accessToken: String, tokenParameterKey: String){
        SerenityHelpers.clearHttpException()
        registerTheDeviceForPushNotifications(authToken = accessToken)
        val errorResponse = SerenityHelpers.getHttpException()
        Assert.assertNotNull(
                "An exception was expected but was not returned within the expected time limit. " +
                        "Access Token invalid value: '$tokenParameterKey",
                errorResponse
        )
        Assert.assertEquals("Incorrect status code returned. " +
                "Access Token invalid value: '$tokenParameterKey",
                HttpStatus.SC_UNAUTHORIZED,
                errorResponse!!.statusCode)
    }

    private fun registerTheDeviceForPushNotifications(authToken: String?) {
        val request =
                PushNotificationsSerenityHelpers.REGISTER_REQUEST.getOrFail<RegisterUserDevicesRequest>()
        try {
            val response = sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .userDevices
                    .post(request, authToken)

            PushNotificationsSerenityHelpers.REGISTER_RESPONSE.set(response)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
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

    @Then("^the device registration is available in the database$")
    fun theDeviceRegistrationIsAvailableInTheDatabase() {
        //Cache has been cleared in data setup.
        MongoDBConnection.UserDevicesCollection.assertNumberOfDocuments(1)
        val userDevices = MongoDBConnection.UserDevicesCollection
                .getValues<MongoRepositoryUserDevice>(MongoRepositoryUserDevice::class.java)
        Assert.assertNotNull("User devices", userDevices)
        Assert.assertEquals("Number of user devices", 1, userDevices.count())
        val registeredDevice = userDevices.first()
        Assert.assertNotNull("Registered device id. $registeredDevice", registeredDevice._id)
        Assert.assertNotNull("Registered device nhsLoginId. $registeredDevice", registeredDevice.NhsLoginId)
        Assert.assertNotNull("Registered device registrationId. $registeredDevice", registeredDevice.RegistrationId)
        Assert.assertNotNull("Registered device pnsToken. $registeredDevice", registeredDevice.PnsToken)
        val expectedPns = PushNotificationsSerenityHelpers.EXPECTED_PNS.getOrFail<String>()
        Assert.assertEquals("Registered device pnsToken", expectedPns, registeredDevice.PnsToken)
    }
}
