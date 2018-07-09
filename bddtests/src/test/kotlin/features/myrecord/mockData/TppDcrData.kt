package features.myrecord.mockData

import mocking.tpp.models.*

object TppDcrData {

    fun getDefaultTppDcrData(): RequestPatientRecordReply {

        return RequestPatientRecordReply(event = mutableListOf())
    }

    fun getMultipleDcrEventsForTpp(): RequestPatientRecordReply {

        val tppDcrEvents = mutableListOf<Event>()

        tppDcrEvents.add(Event("2014-03-15T11:12:55.0Z", "Mr General NhsApp",
                "Kainos GP Demo Unit (General Practice)", mutableListOf(
                EventItem("Medication Template", "Alimemazine 10mg tablets - 1 pack of 28 tablet(s) - [08:00-1][12:00-1][16:00-1][22:00-1]"),
                EventItem("Medication", "Benzoin tincture - 500 ml - use as directed")
        )))

        tppDcrEvents.add(Event("2018-02-16T12:03:23.0Z", "Mr General NhsApp",
                "Kainos GP Demo Unit (General Practice)", mutableListOf(
                EventItem("Medication Template", "Alimemazine 50mg tablets - 1 pack of 14 tablet(s) - [08:00-1][12:00-1][16:00-1][22:00-1]"),
                EventItem("Medication", "(R) Benzoin tincture - 250 ml - use as directed")
        )))

        return RequestPatientRecordReply(
                event = tppDcrEvents)
    }
}
