package features.appointments.stepDefinitions.factories

import features.appointments.steps.AvailableAppointmentsSteps
import mocking.emis.appointments.GetAppointmentSlotsMetaResponseModel
import mocking.emis.models.Location
import mocking.emis.models.Session
import mocking.emis.models.SessionHolder
import mocking.emis.models.SessionType
import mockingFacade.appointments.AppointmentSessionFacade
import mockingFacade.appointments.AppointmentSlotFacade
import mockingFacade.appointments.AppointmentSlotsResponseFacade
import models.Patient
import net.serenitybdd.core.Serenity
import java.time.Duration
import java.util.*

class AppointmentsBookingFactoryEmis: AppointmentsBookingFactory("EMIS"){


    val defaultMetaSlotLocations = arrayListOf(
            Location(1, "Leeds"),
            Location(2, "Sheffield")
    )

    val defaultMetaSlotSessionHolders = arrayListOf(
            SessionHolder(101, "Dr. Who"),
            SessionHolder(102, "Dr. Scott")
    )

    val defaultMetaSlotSessions = arrayListOf(
            Session(
                    "Clinic",
                    301,
                    1,
                    10,
                    SessionType.Timed,
                    1,
                    arrayListOf(101),
                    startDate,
                    endDate
            ),
            Session(
            "Clinic",
            302,
            2,
            10,
            SessionType.Timed,
            1,
            arrayListOf(102),
            startDate,
            endDate
            ))

    override fun generate(){generateEmis()}

    private fun generateEmis(
            emisSlotSessions: ArrayList<Session> = defaultMetaSlotSessions,
            emisAppointmentSessions: ArrayList<AppointmentSessionFacade> = defaultAppointmentSessions,
            emisAppointmentSlots: ArrayList<AppointmentSlotFacade> = defaultAppointmentSlots,
            delayedInSeconds: Long = 0
    ) {
        Serenity.setSessionVariable(AvailableAppointmentsSteps.EXPECTED_SESSIONS_KEY).to(emisSlotSessions)
        Serenity.setSessionVariable(AvailableAppointmentsSteps.EXPECTED_APPOINTMENT_SESSIONS_KEY).to(emisAppointmentSessions)
        Serenity.setSessionVariable(AvailableAppointmentsSteps.EXPECTED_APPOINTMENT_SLOTS_KEY).to(emisAppointmentSlots)

        var patient = Serenity.sessionVariableCalled<Patient>(Patient::class)

        val getAppointmentSlotsResponseModel = AppointmentSlotsResponseFacade(emisAppointmentSessions)
        generateAppointmentSlotResponse(patient, getAppointmentSlotsResponseModel, delayedInSeconds)

        generateStubForMetaAppointmentSlotRequest(
                patient,
                defaultMetaSlotLocations,
                defaultMetaSlotSessionHolders,
                emisSlotSessions,
                delayedInSeconds
        )
    }

    private fun generateStubForMetaAppointmentSlotRequest(
            patient: Patient,
            emisSlotLocations: java.util.ArrayList<Location>,
            emisSlotSessionHolders: java.util.ArrayList<SessionHolder>,
            emisSlotSessions: java.util.ArrayList<Session>,
            delayedInSeconds: Long
    ) {
        val getAppointmentSlotsMetaResponseModel = GetAppointmentSlotsMetaResponseModel(
                emisSlotLocations,
                emisSlotSessionHolders,
                emisSlotSessions
        )
        mockingClient.forEmis {
            appointmentSlotsMetaRequest(patient)
                    .respondWithSuccess(getAppointmentSlotsMetaResponseModel)
                    .delayedBy(Duration.ofSeconds(delayedInSeconds))
        }
    }
}