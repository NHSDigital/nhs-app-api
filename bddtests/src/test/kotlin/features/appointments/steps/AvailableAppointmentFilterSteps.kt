package features.appointments.steps

import features.appointments.factories.AppointmentsSlotsFactory
import mocking.data.appointments.AppointmentsSlotsExampleBuilderWithExpectations
import mockingFacade.appointments.AppointmentFilterFacade
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.thucydides.core.annotations.Step
import org.junit.Assert.assertEquals
import pages.appointments.AvailableAppointmentsPage
import pages.navigation.HeaderNative

open class AvailableAppointmentFilterSteps {

    private val appointmentTypeDefaultOption = "Select type"
    private val locationDefaultOption = "Select location"
    private val clinicianDefaultOption = "No preference"

    lateinit var availableAppointmentsPage: AvailableAppointmentsPage
    lateinit var headerNative: HeaderNative

    @Step
    fun verifyThatAppointmentTypesFilterExistsAndIsCorrectlyPopulated() {
        val expected = Serenity.sessionVariableCalled<List<String>>(
                AppointmentsSlotsExampleBuilderWithExpectations
                        .AppointmentSlotSerenityKeys
                        .EXPECTED_APPOINTMENT_TYPE_KEY
        )

        availableAppointmentsPage.appointmentTypeFilter.assertContents(
                expected.plus(appointmentTypeDefaultOption) as ArrayList<String>
        )

        verifyThatNoAppointmentTypesIsSelected()
    }

    @Step
    fun verifyThatLocationsFilterExistsAndIsCorrectlyPopulated() {
        val expectedLocations =
                Serenity.sessionVariableCalled<List<String>>(
                        AppointmentsSlotsExampleBuilderWithExpectations
                                .AppointmentSlotSerenityKeys
                                .EXPECTED_APPOINTMENT_LOCATIONS_KEY
                )

        availableAppointmentsPage.locationFilter.assertContents(
                expectedLocations.plus(locationDefaultOption) as ArrayList<String>
        )

        assertEquals(
                "Incorrect location option currently selected. ",
                locationDefaultOption,
                availableAppointmentsPage.locationFilter.getSelectedValue()
        )
    }

    @Step
    fun verifyThatCliniciansFilterExistsAndIsCorrectlyPopulated() {
        val expectedClinicians =
                sessionVariableCalled<List<String>>(
                        AppointmentsSlotsExampleBuilderWithExpectations
                                .AppointmentSlotSerenityKeys
                                .EXPECTED_APPOINTMENT_CLINICIANS_KEY
                )

        availableAppointmentsPage.clinicianFilter.assertContents(
                expectedClinicians.plus(clinicianDefaultOption) as ArrayList<String>
        )

        verifyThatNoSpecificClinicianIsSelected()
    }

    @Step
    fun verifyThatTimePeriodFilterExistsAndIsCorrectlyPopulated() {
        availableAppointmentsPage.timePeriodFilter.assertContents(
                arrayListOf(
                        TODAY_OPTION,
                        TOMORROW_OPTION,
                        THIS_WEEK_OPTION,
                        NEXT_WEEK_OPTION,
                        ALL_OPTION
                )
        )

        verifyThatTimePeriodIsSetAsTheDefault()
    }

    fun verifyThatTheFiltersAreNotDisplayed() {
        availableAppointmentsPage.appointmentTypeFilter.assertNotPresent()
        availableAppointmentsPage.locationFilter.assertNotPresent()
        availableAppointmentsPage.clinicianFilter.assertNotPresent()
        availableAppointmentsPage.timePeriodFilter.assertNotPresent()
    }

    @Step
    fun verifyThatNoAppointmentTypesIsSelected() {
        assertEquals(
                "Incorrect appointment type option currently selected. ",
                appointmentTypeDefaultOption,
                availableAppointmentsPage.appointmentTypeFilter.getSelectedValue()
        )
    }

    @Step
    fun verifyThatLocationIsSelected() {
        val locations = Serenity.sessionVariableCalled<List<String>>(
                AppointmentsSlotsExampleBuilderWithExpectations
                        .AppointmentSlotSerenityKeys
                        .EXPECTED_APPOINTMENT_LOCATIONS_KEY
        )
        assertEquals("Test setup incorrect, expected only one location", 1, locations.count())
        assertEquals(
                "Incorrect location option currently selected. ",
                locations.first(),
                availableAppointmentsPage.locationFilter.getSelectedValue()
        )
    }

    @Step
    fun verifyThatNoSpecificClinicianIsSelected() {
        assertEquals(
                "Incorrect clinicians option currently selected. ",
                clinicianDefaultOption,
                availableAppointmentsPage.clinicianFilter.getSelectedValue()
        )
    }

    @Step
    fun verifyThatTimePeriodIsSetAsTheDefault() {
        assertEquals(
                "Incorrect time period option currently selected. ",
                THIS_WEEK_OPTION,
                availableAppointmentsPage.timePeriodFilter.getSelectedValue()
        )
    }

    @Step
    fun selectFilterOptionsToRevealSlots() {
        val filterValues = sessionVariableCalled<AppointmentFilterFacade>(
                AppointmentsSlotsFactory.Expectations.EXPECTED_UI_REPRESENTATION_OF_FILTERED_APPOINTMENTS
        )
        if (!filterValues.type.isNullOrEmpty())
            availableAppointmentsPage.appointmentTypeFilter.selectByText(filterValues.type!!)
        if (!filterValues.location.isNullOrEmpty())
            availableAppointmentsPage.locationFilter.selectByText(filterValues.location!!)
        if (!filterValues.doctor.isNullOrEmpty())
            availableAppointmentsPage.clinicianFilter.selectByText(filterValues.doctor!!)
    }

    fun selectOptionsToRevealSlots() {
        selectFilterOptionsToRevealSlots()
        availableAppointmentsPage.timePeriodFilter.selectByText(ALL_OPTION)

    }

    companion object {
        const val TODAY_OPTION = "Today"
        const val TOMORROW_OPTION = "Tomorrow"
        const val THIS_WEEK_OPTION = "This week"
        const val NEXT_WEEK_OPTION = "Next week"
        const val ALL_OPTION = "All available"
    }
}
