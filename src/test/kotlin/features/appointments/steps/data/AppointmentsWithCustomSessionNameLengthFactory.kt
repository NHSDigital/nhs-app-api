package features.appointments.steps.data

import models.Slot
import java.time.Instant
import java.time.ZoneId
import java.time.format.DateTimeFormatter

class AppointmentsWithCustomSessionNameLengthFactory {

    private val date = Instant.now().atZone(ZoneId.of("UTC"))

    fun mockEmisWithLongSessionName() {

    }

    fun createExpectedSlotsWithLongSessionName(): List<Slot> {
        return createExpectedSlots("Wiltshire, Surrey, South")
    }

    fun mockEmisWithShortSessionName() {

    }

    fun createExpectedSlotsWithShortSessionName(): List<Slot> {
        return createExpectedSlots("Wiltshire, Surrey, South")
    }

    private fun createExpectedSlots(session: String): List<Slot> {
        var slot = Slot()
        slot.date = date.withZoneSameInstant(ZoneId.of("Europe/London")).format(DateTimeFormatter.ofPattern("EEEE dd MMMM yyyy"))
        slot.time = date.withZoneSameInstant(ZoneId.of("Europe/London")).format(DateTimeFormatter.ofPattern("H:m a"))
        slot.session = session
        slot.clinictian.add("Dr Who")
        slot.location = "Leeds"

        return listOf(slot)
    }
}
