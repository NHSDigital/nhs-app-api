package features.appointments.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import features.appointments.steps.AppointmentsSteps
import features.appointments.steps.data.AppointmentsInBSTAndGMTtimeZoneFactory
import features.appointments.steps.data.AppointmentsWithCustomClinicianNameLengthFactory
import features.appointments.steps.data.AppointmentsWithCustomLocationNameLengthFactory
import features.appointments.steps.data.AppointmentsWithCustomSessionNameLengthFactory
import features.authentication.steps.LoginSteps
import features.sharedSteps.BrowserSteps
import features.sharedSteps.NavigationSteps
import mocking.MockDefaults
import mocking.MockingClient
import mocking.emis.appointments.GetAppointmentSlotsMetaResponseModel
import mocking.emis.appointments.GetAppointmentSlotsResponseModel
import mocking.emis.models.*
import mocking.emis.models.appointmentslots.MetaSession
import models.Patient
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import org.mockito.internal.matchers.GreaterThan
import org.openqa.selenium.By


class AppointmentsStepDefinitions {

    @Steps
    lateinit var browser: BrowserSteps
    @Steps
    lateinit var login: LoginSteps
    @Steps
    lateinit var navigation: NavigationSteps
    @Steps
    lateinit var appointments: AppointmentsSteps


    private lateinit var patient: Patient
    private lateinit var slotsModel: GetAppointmentSlotsResponseModel
    private lateinit var metaModel: GetAppointmentSlotsMetaResponseModel


    @Given("^I am on the appointments page")
    fun iAmOnTheAppointmentsPage() {
        browser.goToApp()
        login.asDefault()
        this.patient = MockDefaults.patient
        navigation.select("appointments")
    }


    @Then("^I see the expected slotsModel$")
    fun iSeeAvailableAppointmentSlotsForTheNextTwoWeeks() {
        // appointments.slots(Equals(this.slotsModel.sessions[0].slots))
        Assert.fail("Not implemented yet")
    }


    /*************************************************/
    // STEPS COPIED FROM OLD REPO -- needs cleaning
    /*************************************************/

    val mockingClient = MockingClient.instance
    val availableAppointmentsPage = appointments


    @And("^there are available appointment slots$")
    fun there_are_available_appointment_slots() {
        val expectedSlots = createAppointmentSlotsResponse()
        val expectedMeta = createAppointmentSlotMetaModel()
        setExpectedSlots(expectedSlots)
        setExpectedMeta(expectedMeta)

        mockingClient
                .forEmis {
                    appointmentSlotsRequest(patient)
                            .respondWithSuccess(expectedSlots)
                }

        mockingClient
                .forEmis {
                    appointmentSlotsMetaRequest(patient)
                            .respondWithSuccess(expectedMeta)
                }
    }

    @Given("^there are available appointment slots with long location name$")
    fun there_are_available_appointment_slots_with_long_location_name() {

        val model = createAppointmentSlotMetaModel()
        model.locations[0].locationName = "Extra long location name that will be truncated"

        mockingClient
                .forEmis {
                    appointmentSlotsRequest(patient)
                            .respondWithSuccess(createAppointmentSlotsResponse())
                }

        mockingClient
                .forEmis {
                    appointmentSlotsMetaRequest(patient)
                            .respondWithSuccess(model)
                }
    }

    @Given("^there are available appointment slots with location name length less or equal 24 characters$")
    fun there_are_available_appointment_slots_with_location_name_length_less_or_equal_24_characters() {
        val model = createAppointmentSlotMetaModel()
        model.locations[0].locationName = "Location with 24 chars."

        mockingClient
                .forEmis {
                    appointmentSlotsRequest(patient)
                            .respondWithSuccess(createAppointmentSlotsResponse())
                }

        mockingClient
                .forEmis {
                    appointmentSlotsMetaRequest(patient)
                            .respondWithSuccess(model)
                }
    }

    @Given("^there are available appointment slots with long clinician name$")
    fun there_are_available_appointment_slots_with_long_clinician_name() {
        AppointmentsWithCustomClinicianNameLengthFactory().mockEmisWithLongClinicianName()
    }

    @Given("^there are available appointment slots with the Clinician Name length less or equal than 24 characters$")
    fun there_are_available_appointment_slots_with_the_Clinician_Name_length_less_or_equal_than_24_characters() {
        AppointmentsWithCustomClinicianNameLengthFactory().mockEmisWithShortClinicianName()
    }

    @Given("^there are available appointment slots with long session name$")
    fun there_are_available_appointment_slots_with_long_session_name() {
        AppointmentsWithCustomSessionNameLengthFactory().mockEmisWithLongSessionName()
    }

    @Given("^there are available appointment slots with the AppointmentSlotSession Name length less or equal than 24 characters$")
    fun there_are_available_appointment_slots_with_the_Session_Name_length_less_or_equal_than_24_characters() {
        AppointmentsWithCustomSessionNameLengthFactory().mockEmisWithShortSessionName()
    }

    @Given("^there are available appointment slots with some in BST and some in GMT$")
    fun there_are_available_appointment_slots_with_some_in_BST_and_some_in_GMT() {
        AppointmentsInBSTAndGMTtimeZoneFactory().mockEmis()
    }

    @Given("^there are no available slots$")
    fun there_are_no_available_slots() {

    }

    @Then("^I see available appointment slots for the next 2 weeks$")
    fun i_see_available_appointment_slots_for_the_next_2_weeks() {

    }

    @Then("^the appointment slots are ordered ascending start date and time then first clinician name$")
    fun the_appointment_slots_are_ordered_ascending_start_date_and_time_then_first_clinician_name() {

    }

    @Then("^I see available slots display date in correct format$")
    fun i_see_available_slots_display_date_in_correct_format() {
        var dayOfTheWeek = "(Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday)"
        var months = "(January|February|March|April|May|June|July|August|September|October|November|December)"
        var correctFormat = "^$dayOfTheWeek\\s([0-9]{2})\\s$months\\s([0-9]{4})\$"
        var slots = availableAppointmentsPage.appointments.getAllSlots()

        Assert.assertThat(slots.size, GreaterThan(0))

        slots.forEach { slot ->
            var hasCorrectFormat = Regex(correctFormat).matches(slot.date)
            Assert.assertTrue(hasCorrectFormat)
        }
    }

    @Then("^I see available slots display start time in correct format includes AM or PM$")
    fun i_see_available_slots_display_start_time_in_correct_format_includes_AM_or_PM() {
        var correctFormat = "^([01][0-9]|2[0-3]):[0-5][0-9]\\s(AM|PM)\$"
        var slots = availableAppointmentsPage.appointments.getAllSlots()

        Assert.assertThat(slots.size, GreaterThan(0))

        slots.forEach { slot ->
            var hasCorrectFormat = Regex(correctFormat).matches(slot.time)
            Assert.assertTrue(hasCorrectFormat)
        }
    }

    @Then("^each slot displays the start time in the timezone effective on that date$")
    fun each_slot_displays_the_start_time_in_the_timezone_effective_on_that_date() {
        val expectedSlots = AppointmentsInBSTAndGMTtimeZoneFactory().createExpectedSlots()
        val actualSlots = availableAppointmentsPage.appointments.getAllSlots()

        Assert.assertArrayEquals(expectedSlots.toTypedArray(), actualSlots.toTypedArray())
    }

    @Then("^I see appropriate information message when no slots are available$")
    fun i_see_appropriate_information_message_when_no_slots_are_available() {
        var message = availableAppointmentsPage.appointments.getInformationMessage()
        Assert.assertEquals("There are no appointments available at the moment", message.findElement(By.cssSelector("h3")).text)
    }

    @Given("^GP system is unavailable$")
    fun gp_system_is_unavailable() {

    }

    @Then("^I see appropriate information message when GP system is unavailable$")
    fun i_see_appropriate_information_message_when_GP_system_is_unavailable() {
        var message = availableAppointmentsPage.appointments.getInformationMessage()
        Assert.assertEquals("GP system is unavailable", message.findElement(By.cssSelector("h3")).text)
    }

    @Then("^I see available slots with the location length greater than 24 characters is truncated$")
    fun i_see_available_slots_with_the_location_length_greater_than_24_characters_is_truncated() {
        var expectedSlots = AppointmentsWithCustomLocationNameLengthFactory(mockingClient).createExpectedSlotsWithLongLocationName()
        var actualSlots = availableAppointmentsPage.appointments.getAllSlots()

        Assert.assertArrayEquals(expectedSlots.toTypedArray(), actualSlots.toTypedArray())
    }

    @Then("^I see available slots with the location length less or equal than 24 characters is shown in full$")
    fun i_see_available_slots_with_the_location_length_less_or_equal_than_24_characters_is_shown_in_full() {

    }

    @Then("^I see available slots with the Clinician Name length greater than 24 characters is truncated$")
    fun i_see_available_slots_with_the_Clinician_Name_length_greater_than_24_characters_is_truncated() {
        var expectedSlots = AppointmentsWithCustomClinicianNameLengthFactory().createExpectedSlotsWithLongClinicianName()
        var actualSlots = availableAppointmentsPage.appointments.getAllSlots()

        Assert.assertArrayEquals(expectedSlots.toTypedArray(), actualSlots.toTypedArray())
    }

    @Then("^I see available slots with the Clinician Name length less or equal than 24 characters is shown in full$")
    fun i_see_available_slots_with_the_Clinician_Name_length_less_or_equal_than_24_characters_is_shown_in_full() {
        var expectedSlots = AppointmentsWithCustomClinicianNameLengthFactory().createExpectedSlotsWithShortClinicianName()
        var actualSlots = availableAppointmentsPage.appointments.getAllSlots()

        Assert.assertArrayEquals(expectedSlots.toTypedArray(), actualSlots.toTypedArray())
    }

    @Then("^I see available slots with the AppointmentSlotSession Name length greater than 24 characters is truncated$")
    fun i_see_available_slots_with_the_Session_Name_length_greater_than_24_characters_is_truncated() {
        var expectedSlots = AppointmentsWithCustomSessionNameLengthFactory().createExpectedSlotsWithLongSessionName()
        var actualSlots = availableAppointmentsPage.appointments.getAllSlots()

        Assert.assertArrayEquals(expectedSlots.toTypedArray(), actualSlots.toTypedArray())
    }

    @Then("^I see available slots with the AppointmentSlotSession Name length less or equal 24 characters is shown in full$")
    fun i_see_available_slots_with_the_Session_Name_length_less_or_equal_24_characters_is_shown_in_full() {
        var expectedSlots = AppointmentsWithCustomSessionNameLengthFactory().createExpectedSlotsWithShortSessionName()
        var actualSlots = availableAppointmentsPage.appointments.getAllSlots()

        Assert.assertArrayEquals(expectedSlots.toTypedArray(), actualSlots.toTypedArray())
    }


    private fun getExpectedSlots(): ArrayList<AppointmentSlot> {
        return this.slotsModel.sessions[0].slots
    }

    private fun getExpectedMeta(): GetAppointmentSlotsMetaResponseModel {
        return this.metaModel
    }

    private fun setExpectedMeta(meta: GetAppointmentSlotsMetaResponseModel) {
        this.metaModel = meta
    }

    private fun setExpectedSlots(slots: GetAppointmentSlotsResponseModel) {
        this.slotsModel = slots
    }

    private fun createAppointmentSlotMetaModel(): GetAppointmentSlotsMetaResponseModel {
        val locations = arrayListOf(
                Location(
                        locationId = 1,
                        locationName = "Some surgery",
                        numberAndStreet = "12 Some Street",
                        village = "A Village",
                        town = "N E Town",
                        postcode = "ANY12 2NE"
                )
        )

        val sessionHolders = arrayListOf(
                SessionHolder(
                        clinicianId = 1,
                        displayName = "Mr. Frank Pickles",
                        forenames = "Frank Timothy",
                        surname = "Pickles",
                        title = "Mr",
                        sex = Sex.Male,
                        jobRole = "Nurse"
                )
        )

        val sessions = arrayListOf(
                MetaSession(
                        sessionId = 1,
                        sessionName = "Ear syringing",
                        locationId = 1,
                        defaultDuration = 20,
                        sessionType = SessionType.Untimed,
                        numberOfSlots = 4,
                        clinicianIds = listOf(1)
                ),
                MetaSession(
                        sessionName = "GP consultation",
                        sessionId = 2,
                        locationId = 1,
                        defaultDuration = 15,
                        sessionType = SessionType.Timed,
                        numberOfSlots = 2,
                        clinicianIds = listOf(2)
                )
        )

        return GetAppointmentSlotsMetaResponseModel(
                locations = locations,
                sessionHolders = sessionHolders,
                sessions = sessions
        )
    }

    private fun createAppointmentSlotsResponse(): GetAppointmentSlotsResponseModel {
        return GetAppointmentSlotsResponseModel(
                arrayListOf(
                        AppointmentSession(
                                slots = arrayListOf(
                                        AppointmentSlot(
                                                slotId = 1,
                                                startTime = "2018-05-08T13:00:00.000Z",
                                                endTime = "2018-05-08T13:15:00.000Z",
                                                slotTypeName = "GP appointment",
                                                slotTypeStatus = SlotTypeStatus.Practice
                                        ),
                                        AppointmentSlot(
                                                slotId = 2,
                                                startTime = "2018-05-08T13:00:00.000Z",
                                                endTime = "2018-05-08T13:15:00.000Z",
                                                slotTypeName = "Hearing test",
                                                slotTypeStatus = SlotTypeStatus.Practice
                                        )
                                )
                        )
                )
        )
    }
}