package mocking.vision.models.appointments

import mocking.vision.models.appointments.BookedAppointmentsResponse.Companion.name
import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(namespace = "urn:vision", name = name)
@XmlAccessorType(XmlAccessType.FIELD)
data class BookedAppointmentsResponse(
        @XmlElement(namespace = "urn:vision", name = "slot")
        var slots: Slots? = null,
        @XmlElement(namespace = "urn:vision")
        var references: References? = null,
        @XmlElement(namespace = "urn:vision", name = "settings")
        var settings: AppointmentSettings? = null
) {

    companion object {
        const val name = "bookedAppointmentsResponse"
    }
}
