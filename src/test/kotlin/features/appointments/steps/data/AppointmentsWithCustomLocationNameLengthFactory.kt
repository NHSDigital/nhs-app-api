package features.appointments.steps.data

import mocking.MockingClient
import models.Slot
import java.time.Instant
import java.time.ZoneId
import java.time.format.DateTimeFormatter

class AppointmentsWithCustomLocationNameLengthFactory(val mockingClient: MockingClient) {

    private val date = Instant.now().atZone(ZoneId.of("UTC"))

    fun createExpectedSlotsWithLongLocationName(): List<Slot> {
        return createExpectedSlots("Wiltshire, Surrey, South")
    }

    private fun createExpectedSlots(location: String): List<Slot> {
        var slot = Slot()
        slot.date = date.withZoneSameInstant(ZoneId.of("Europe/London")).format(DateTimeFormatter.ofPattern("EEEE dd MMMM yyyy"))
        slot.time = date.withZoneSameInstant(ZoneId.of("Europe/London")).format(DateTimeFormatter.ofPattern("H:m a"))
        slot.session = "Appointment Session"
        slot.clinician.add("Dr Who")
        slot.location = location

        return listOf(slot)
    }
}