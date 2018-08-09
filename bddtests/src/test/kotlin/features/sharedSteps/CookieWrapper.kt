package features.sharedSteps

import com.google.gson.Gson
import org.junit.Assert

data class CookieWrapper private constructor(
        private val header : CookieHeaderWrapper,
        private val availableAppointments : CookieAvailableAppointmentsWrapper,
        private val auth : CookieAuthWrapper,
        private val device: CookieDeviceWrapper,
        private val http: CookieHttpWrapper,
        private val navigation: CookieNavigationWrapper,
        private val prescriptions : CookiePrescriptionsWrapper,
        private val repeatPrescriptionCourses: CookieRepeatPrescriptionCoursesWrapper,
        private val session : CookieSessionWrapper,
        private val errors: CookieErrorsWrapper,
        private val myAppointments: CookieMyAppointmentsWrapper,
        private val flashMessage: CookieFlashMessageWrapper
) {
    fun isLoggedIn():Boolean {
        return this.auth.loggedIn;
    }

    fun assertIsLoggedOut() {

        var expectedAuth = CookieAuthWrapper(loggedIn = false,
                config = CookieEmptyObjectWrapper(empty = null),
                user = CookieEmptyObjectWrapper(empty = null),
                authorised = true)
        this.auth.AssertEquals(expectedAuth)

        var expectedAppointments = CookieMyAppointmentsWrapper(
                appointmentSessions = arrayListOf(),
                clinicians = arrayListOf(),
                locations = arrayListOf(),
                appointments = arrayListOf(),
                cancellationReasons = arrayListOf(),
                selectedAppointment = null,
                hasLoaded = false,
                hasErrored = false)

        this.myAppointments.AssertEquals(expectedAppointments)

        var expectedPrescriptions = CookiePrescriptionsWrapper(
                prescriptionCourses = arrayListOf(),
                hasLoaded = false,
                hasErrored = false)

        this.prescriptions.AssertEquals(expectedPrescriptions)

        var expectedRepeatPrescriptions = CookieRepeatPrescriptionCoursesWrapper(
                courses = arrayListOf(),
                repeatPrescriptionCourses = arrayListOf(),
                specialRequest = null,
                hasLoaded = false,
                hasErrored = false,
                validated = false,
                isValid = false
        )
        this.repeatPrescriptionCourses.AssertEquals(expectedRepeatPrescriptions)

        var mySession = this.session;
        Assert.assertNull(mySession.lastCalledAt);
    }

    companion object {

        fun fromJson(json: String): CookieWrapper {
            val targetObject = Gson().fromJson(json, CookieWrapper::class.java)
            Assert.assertTrue("cookie serialisation", targetObject != null)
            return targetObject
        }
    }

    private data class CookieHeaderWrapper(val headerText: String)


    private data class CookieAvailableAppointmentsWrapper(
            val slots: ArrayList<String>,
            val filteredSlots: ArrayList<String>,
            val hasLoaded: Boolean,
            val selectedSlot: String?,
            val booked: Boolean,
            val filtersOptions: CookieFiltersOptionsWrapper,
            val selectedOptions: CookieSelectedOptionsWrapper)

    private data class CookieFiltersOptionsWrapper(
            val types: ArrayList<String>,
            val locations: ArrayList<String>,
            val clinicians: ArrayList<String>,
            val dates: ArrayList<String>
    )

    private data class CookieSelectedOptionsWrapper(
            val type: String,
            val location: String,
            val clinician: String,
            val date: String)

    private data class CookieAuthWrapper(
            val loggedIn: Boolean,
            val config: CookieEmptyObjectWrapper?,
            val user: CookieEmptyObjectWrapper?,
            val authorised: Boolean
    ) {
        fun AssertEquals(expectedAuth: CookieAuthWrapper) {
            Assert.assertEquals("auth.loggedIn", expectedAuth.loggedIn, loggedIn)
            Assert.assertEquals("auth.config", expectedAuth.config, config)
            Assert.assertEquals("auth.user", expectedAuth.user, user)
            Assert.assertEquals("auth.authorised", expectedAuth.authorised, authorised)
        }
    }

    private data class CookieDeviceWrapper(
            val isNativeApp: Boolean
    )

    private data class CookieHttpWrapper(
            val isLoading: Boolean,
            val cancelRequestHandlers: ArrayList<String>
    )

    private data class CookieEmptyObjectWrapper(
            val empty: String?
    )

    private data class CookieNavigationWrapper(
            val menuItemStatusAt: ArrayList<Boolean>
    )

    private data class CookiePrescriptionsWrapper(
            val prescriptionCourses: ArrayList<String>,
            val hasLoaded: Boolean,
            val hasErrored: Boolean) {
        fun AssertEquals(expected: CookiePrescriptionsWrapper) {
            Assert.assertEquals("prescriptions.prescriptionCourses", expected.prescriptionCourses, this.prescriptionCourses)
            Assert.assertEquals("prescriptions.hasLoaded", expected.hasLoaded, this.hasLoaded)
            Assert.assertEquals("prescriptions.hasErrored", expected.hasErrored, this.hasErrored)
        }
    }

    private data class CookieRepeatPrescriptionCoursesWrapper(
            val courses: ArrayList<String>,
            val repeatPrescriptionCourses: ArrayList<String>,
            val specialRequest: CookieEmptyObjectWrapper?,
            val hasLoaded: Boolean,
            val hasErrored: Boolean,
            val validated: Boolean,
            val isValid: Boolean
    ) {
        fun AssertEquals(expected: CookieRepeatPrescriptionCoursesWrapper) {
            Assert.assertEquals("repeatPrescriptionCourses.courses", expected.courses, this.courses)
            Assert.assertEquals("repeatPrescriptionCourses.repeatPrescriptionCourses", expected.repeatPrescriptionCourses, this.repeatPrescriptionCourses)
            Assert.assertEquals("repeatPrescriptionCourses.specialRequest", expected.specialRequest, this.specialRequest)
            Assert.assertEquals("repeatPrescriptionCourses.hasLoaded", expected.hasLoaded, this.hasLoaded)
            Assert.assertEquals("repeatPrescriptionCourses.hasErrored", expected.hasErrored, this.hasErrored)
            Assert.assertEquals("repeatPrescriptionCourses.validated", expected.validated, this.validated)
            Assert.assertEquals("repeatPrescriptionCourses.isValid", expected.isValid, this.isValid)
        }
    }

    private data class CookieSessionWrapper(val lastCalledAt: String)

    private data class CookieErrorsWrapper(
    val showApiError: Boolean,
     val apiErrors: ArrayList<String>,
     val pageSettings: CookiePageSettingsWrapper,
     val hasConnectionProblem: Boolean,
     val routePath: String
    )

    private data class CookiePageSettingsWrapper(
            val redirectUrl: String,
            val showApiError: Boolean,
            val ignoredErrors: ArrayList<String>)

    private data class CookieMyAppointmentsWrapper(
            val appointmentSessions: ArrayList<String>,
            val clinicians: ArrayList<String>,
            val locations: ArrayList<String>,
            val appointments: ArrayList<String>,
            val cancellationReasons: ArrayList<String>,
            val selectedAppointment: CookieEmptyObjectWrapper?,
            val hasLoaded: Boolean,
            val hasErrored: Boolean) {

        fun AssertEquals(expectedAppointments: CookieMyAppointmentsWrapper) {
            Assert.assertEquals("myAppointments.appointmentSessions", expectedAppointments.appointmentSessions, this.appointmentSessions)
            Assert.assertEquals("myAppointments.clinicians", expectedAppointments.clinicians, this.clinicians)
            Assert.assertEquals("myAppointments.locations", expectedAppointments.locations, this.locations)
            Assert.assertEquals("myAppointments.appointments", expectedAppointments.appointments, this.appointments)
            Assert.assertEquals("myAppointments.cancellationReasons", expectedAppointments.cancellationReasons, this.cancellationReasons)
            Assert.assertEquals("myAppointments.selectedAppointment", expectedAppointments.selectedAppointment, this.selectedAppointment)
            Assert.assertEquals("myAppointments.hasLoaded", expectedAppointments.hasLoaded, this.hasLoaded)
            Assert.assertEquals("myAppointments.hasErrored", expectedAppointments.hasErrored, this.hasErrored)

        }
    }

    private data class CookieFlashMessageWrapper(
            val message: String,
            val hasBeenShown: Boolean,
            val show: Boolean,
            val type: String)

}
