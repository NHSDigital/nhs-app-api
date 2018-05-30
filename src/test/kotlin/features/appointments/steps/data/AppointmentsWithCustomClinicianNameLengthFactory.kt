package features.appointments.steps.data

import models.Slot
import java.time.Instant
import java.time.ZoneId
import java.time.format.DateTimeFormatter

class AppointmentsWithCustomClinicianNameLengthFactory {
    private val date = Instant.now().atZone(ZoneId.of("UTC"))

    fun mockEmisWithLongClinicianName() {
    }

    fun createExpectedSlotsWithLongClinicianName(): List<Slot> {
        return createExpectedSlots(arrayListOf("Dr Emmett Brown", "Doctor of Medicine Grego"))
    }

    fun mockEmisWithShortClinicianName() {
    }

    fun createExpectedSlotsWithShortClinicianName(): List<Slot> {
        return createExpectedSlots(arrayListOf("Dr Emmett Brown", "Dr Who"))
    }

    private fun createExpectedSlots(clinicians: ArrayList<String>): List<Slot> {
        var slot = Slot()
        slot.date = date.withZoneSameInstant(ZoneId.of("Europe/London")).format(DateTimeFormatter.ofPattern("EEEE dd MMMM yyyy"))
        slot.time = date.withZoneSameInstant(ZoneId.of("Europe/London")).format(DateTimeFormatter.ofPattern("H:m a"))
        slot.session = "Appointment Session"
        slot.clinician = clinicians
        slot.location = "Leeds"

        return listOf(slot)
    }
}
