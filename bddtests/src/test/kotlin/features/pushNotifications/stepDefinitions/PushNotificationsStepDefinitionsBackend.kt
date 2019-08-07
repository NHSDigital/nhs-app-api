package features.pushNotifications.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity.sessionVariableCalled
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
    fun iAmAUserWishingToRegisterTheirDeviceForPushNotifications(gpSystem: String) {
        SerenityHelpers.setGpSupplier(gpSystem)
        val patient = Patient.getDefault(gpSystem)
        SerenityHelpers.setPatient(patient)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)
        val deviceType = "Android"
        val devicePns = "devicePns1234"
        PushNotificationsSerenityHelpers.EXPECTED_DEVICE_TYPE.set(deviceType)
        PushNotificationsSerenityHelpers.REGISTER_REQUEST.set(RegisterUserDevicesRequest(devicePns, deviceType))
    }

    @When("^I register the device for push notifications$")
    fun iRegisterTheDeviceForPushNotifications() {
        registerTheDeviceForPushNotifications()
    }

    @When("^I register the device for push notifications without auth token$")
    fun iRegisterTheDeviceForPushNotificationsWithoutAuthToken() {
        registerTheDeviceForPushNotifications(withAuthToken = false)
    }

    private fun registerTheDeviceForPushNotifications( withAuthToken: Boolean = true) {
        val request =
                PushNotificationsSerenityHelpers.REGISTER_REQUEST.getOrFail<RegisterUserDevicesRequest>()
        try {
            val response = sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .userDevices
                    .post(SerenityHelpers.getPatient(), request, withAuthToken)

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
}
