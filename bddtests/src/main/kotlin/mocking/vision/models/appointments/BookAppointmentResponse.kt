package mocking.vision.models.appointments

import mocking.vision.models.appointments.BookedAppointmentsResponse.Companion.name
import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(namespace = "urn:vision", name = name)
@XmlAccessorType(XmlAccessType.FIELD)
data class BookAppointmentResponse(
        @XmlElement(namespace = "urn:vision", name = "slot")
        var slot: Slot? = null
) {

    companion object {
        const val name = "appointment"
    }
}