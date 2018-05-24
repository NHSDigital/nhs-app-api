package features.appointments.steps.data

import models.Slot
import java.time.Instant
import java.time.ZoneId
import java.time.format.DateTimeFormatter

class AppointmentsInBSTAndGMTtimeZoneFactory {

    private val bstDate = Instant.parse("2018-06-14T10:12:30.00Z").atZone(ZoneId.of("UTC"))
    private val gmtDate = Instant.parse("2018-12-21T10:14:20.00Z").atZone(ZoneId.of("UTC"))

    fun mockEmis() {

    }

    fun createExpectedSlots(): List<Slot> {
        var slot1 = Slot()
        slot1.date = bstDate.withZoneSameInstant(ZoneId.of("Europe/London")).format(DateTimeFormatter.ofPattern("EEEE dd MMMM yyyy"))
        slot1.time = bstDate.withZoneSameInstant(ZoneId.of("Europe/London")).format(DateTimeFormatter.ofPattern("H:m a"))
        slot1.session = "Appointment Session"
        slot1.clinictian.add("Dr Emmett Brown")
        slot1.location = "Wiltshire, Surrey, South"

        var slot2 = Slot()
        slot2.date = gmtDate.withZoneSameInstant(ZoneId.of("Europe/London")).format(DateTimeFormatter.ofPattern("EEEE dd MMMM yyyy"))
        slot2.time = gmtDate.withZoneSameInstant(ZoneId.of("Europe/London")).format(DateTimeFormatter.ofPattern("H:m a"))
        slot2.session = "Appointment Session"
        slot2.clinictian.add("Dr Who")
        slot2.location = "Wiltshire, Surrey, South"

        return listOf(slot1, slot2)
    }
}
