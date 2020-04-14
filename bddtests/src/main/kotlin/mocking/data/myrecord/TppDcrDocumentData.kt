package mocking.data.myrecord

import mocking.tpp.models.Event
import mocking.tpp.models.EventItem
import mocking.tpp.models.RequestPatientRecordReply

object TppDcrDocumentData {

    fun getMultipleDcrEventsForTppDcrDocuments(
        hasInvalidType: Boolean = false,
        hasNonViewableType: Boolean = false): RequestPatientRecordReply {
        val tppDcrEvents = mutableListOf<Event>()

        var details = "JPEG: Blood-tests.jpeg - some comments"

        if (hasInvalidType) {
            details = "TGA: Blood-tests.tga - some comments"
        }

        if (hasNonViewableType) {
            details = "PDF: Blood-tests.pdf - some comments"
        }

        tppDcrEvents.add(Event("2018-02-18T11:12:55.0Z", "Mr General NhsApp",
                "Kainos GP Demo Unit (General Practice)", mutableListOf(
                EventItem("Attachment", details,
                        "123456433546"),
                EventItem("Attachment", details,
                        "123456433543")
        )))

        tppDcrEvents.add(Event("2018-02-18T12:03:23.0Z", "Mr General NhsApp",
                "Kainos GP Demo Unit (General Practice)", mutableListOf(
                EventItem("Attachment", details,
                        "123456433541"),
                EventItem("Attachment", details,
                        "123456433548")
        )))

        return RequestPatientRecordReply(
                event = tppDcrEvents)
    }

    fun getMultipleLetterDcrEventsForTppDcrDocuments(): RequestPatientRecordReply {

        val tppDcrEvents = mutableListOf<Event>()

        tppDcrEvents.add(Event("2018-02-18T11:12:55.0Z", "Mr General NhsApp",
                "Kainos GP Demo Unit (General Practice)", mutableListOf(
                EventItem("Letter",
                        "DOCX: Blood-tests.docx - some comments",
                        "123456433546"),
                EventItem("Letter",
                        "DOCX: Blood-tests.docx - some comments",
                        "123456433543")
        )))

        tppDcrEvents.add(Event("2018-02-18T12:03:23.0Z", "Mr General NhsApp",
                "Kainos GP Demo Unit (General Practice)", mutableListOf(
                EventItem("Letter",
                        "DOCX: Blood-tests.docx" +
                                "[08:00-1][12:00-1][16:00-1][22:00-1]",
                        "123456433541"),
                EventItem("Letter",
                        "DOCX: Blood-tests.docx" +
                                "[08:00-1][12:00-1][16:00-1][22:00-1]",
                        "123456433548")
        )))

        return RequestPatientRecordReply(
                event = tppDcrEvents)
    }

    fun getMultipleDcrEventsForTppWithNoDocuments(): RequestPatientRecordReply {

        val tppDcrEvents = mutableListOf<Event>()

        tppDcrEvents.add(Event("2014-03-15T11:12:55.0Z", "Mr General NhsApp",
                "Kainos GP Demo Unit (General Practice)", mutableListOf(
                EventItem("Medication Template",
                        "Alimemazine 10mg tablets - 1 pack of 28 tablet(s) - " +
                                "[08:00-1][12:00-1][16:00-1][22:00-1]"),
                EventItem("Medication", "Benzoin tincture - 500 ml - use as directed")
        )))

        tppDcrEvents.add(Event("2018-02-16T12:03:23.0Z", "Mr General NhsApp",
                "Kainos GP Demo Unit (General Practice)", mutableListOf(
                EventItem("Medication Template",
                        "Alimemazine 50mg tablets - 1 pack of 14 tablet(s) - " +
                                "[08:00-1][12:00-1][16:00-1][22:00-1]"),
                EventItem("Medication", "(R) Benzoin tincture - 250 ml - use as directed")
        )))

        return RequestPatientRecordReply(
                event = tppDcrEvents)
    }
}
