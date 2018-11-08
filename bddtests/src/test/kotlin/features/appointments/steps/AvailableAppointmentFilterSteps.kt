package features.appointments.steps

import mocking.data.appointments.AppointmentsBookingData
import mocking.data.appointments.AppointmentsSlotsExampleBuilderWithExpectations
import mockingFacade.appointments.AppointmentFilterFacade
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.thucydides.core.annotations.Step
import org.junit.Assert
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
import pages.appointments.AvailableAppointmentsPage
import pages.navigation.HeaderNative

open class AvailableAppointmentFilterSteps : AppointmentsBookingData() {

    private val appointmentTypeDefaultOption = "Select type"
    private val locationDefaultOption = "Select location"
    private val clinicianDefaultOption = "No preference"

    lateinit var availableAppointmentsPage: AvailableAppointmentsPage
    lateinit var headerNative: HeaderNative

    @Step
    fun verifyThatAppointmentTypesFilterExistsAndIsCorrectlyPopulated() {
        val actualAppointmentTypeOptions = availableAppointmentsPage.appointmentTypeFilter.getFilterContents()
        assertOptionExists(appointmentTypeDefaultOption, actualAppointmentTypeOptions, "default")

        val expected = Serenity.sessionVariableCalled<ArrayList<String>>(
                AppointmentsSlotsExampleBuilderWithExpectations
                        .AppointmentSlotSerenityKeys
                        .EXPECTED_APPOINTMENT_TYPE_KEY
        )

        expected.forEach { expectedAppointmentType ->
            assertOptionExists(expectedAppointmentType, actualAppointmentTypeOptions) }

        verifyThatNoAppointmentTypesIsSelected()
    }

    @Step
    fun verifyThatLocationsFilterExistsAndIsCorrectlyPopulated() {
        val actualLocationOptions = availableAppointmentsPage.locationFilter.getFilterContents()
        val expectedLocations =
                Serenity.sessionVariableCalled<ArrayList<String>>(
                        AppointmentsSlotsExampleBuilderWithExpectations
                                .AppointmentSlotSerenityKeys
                                .EXPECTED_APPOINTMENT_LOCATIONS_KEY
                )

        for (expectedLocation in expectedLocations) {
            assertOptionExists(expectedLocation, actualLocationOptions)
        }

        assertEquals(
                "Incorrect location option currently selected. ",
                locationDefaultOption,
                availableAppointmentsPage.locationFilter.getSelectedValue()
        )
    }

    @Step
    fun verifyThatCliniciansFilterExistsAndIsCorrectlyPopulated() {
        val actualClinicianOptions = availableAppointmentsPage.clinicianFilter.getFilterContents()
        assertOptionExists(clinicianDefaultOption, actualClinicianOptions, "default")

        val expectedClinicians =
                sessionVariableCalled<ArrayList<String>>(
                        AppointmentsSlotsExampleBuilderWithExpectations
                                .AppointmentSlotSerenityKeys
                                .EXPECTED_APPOINTMENT_CLINICIANS_KEY
                )

        Assert.assertNotNull(
                "Expected session variable 'EXPECTED_APPOINTMENT_CLINICIANS_KEY' to have value",
                expectedClinicians
        )

        for (expectedClinician in expectedClinicians) {
            assertOptionExists(expectedClinician, actualClinicianOptions)

        }
        verifyThatNoSpecificClinicianIsSelected()
    }

    @Step
    fun verifyThatTimePeriodFilterExistsAndIsCorrectlyPopulated() {
        val actualTimePeriodOptions = availableAppointmentsPage.timePeriodFilter.getFilterContents()
        assertOptionExists(TODAY_OPTION, actualTimePeriodOptions)
        assertOptionExists(TOMORROW_OPTION, actualTimePeriodOptions)
        assertOptionExists(THIS_WEEK_OPTION, actualTimePeriodOptions)
        assertOptionExists(NEXT_WEEK_OPTION, actualTimePeriodOptions)
        assertOptionExists(ALL_OPTION, actualTimePeriodOptions)
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
        val locations = Serenity.sessionVariableCalled<ArrayList<String>>(
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
                AppointmentsSlotsExampleBuilderWithExpectations
                        .AppointmentSlotSerenityKeys
                        .EXPECTED_APPOINTMENT_FILTER_FACADE_KEY
        )
        if (!filterValues.type.isNullOrEmpty())
            availableAppointmentsPage.appointmentTypeFilter.selectByText(filterValues.type!!)
        if (!filterValues.location.isNullOrEmpty())
            availableAppointmentsPage.locationFilter.selectByText(filterValues.location!!)
        if (!filterValues.doctor.isNullOrEmpty())
            availableAppointmentsPage.clinicianFilter.selectByText(filterValues.doctor!!)
    }

    private fun assertOptionExists(defaultOption: String, actualOptions: ArrayList<String>, optionType: String = "an") {
        assertTrue(
                String.format("%s not present as %s option", defaultOption, optionType),
                actualOptions.contains(defaultOption)
        )
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
