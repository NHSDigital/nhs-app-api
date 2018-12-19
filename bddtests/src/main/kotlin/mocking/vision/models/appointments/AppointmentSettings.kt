package mocking.vision.models.appointments

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement
import javax.xml.bind.annotation.XmlValue


@XmlRootElement(namespace = "urn:vision", name = "settings")
@XmlAccessorType(XmlAccessType.FIELD)
data class AppointmentSettings(
        @XmlElement(namespace = "urn:vision", name = "cancellationReasons")
        var cancellationReasons: CancellationReasons? = null,
        @XmlElement(namespace = "urn:vision", name = "bookingReason")
        val bookingReason: BookingReason? = null,
        @XmlElement(namespace = "urn:vision", name = "cancellationCutoffMinutes")
        val cancellationCutoffMinutes: Int = 60)

@XmlAccessorType(XmlAccessType.FIELD)
data class BookingReason(
        @XmlElement(namespace = "urn:vision", name = "add")
        val add: Boolean = true,
        @XmlElement(namespace = "urn:vision", name = "display")
        val display: Boolean = true,
        @XmlElement(namespace = "urn:vision", name = "edit")
        val edit: Boolean = true)

@XmlAccessorType(XmlAccessType.FIELD)
data class CancellationReasons(
        @XmlElement(namespace = "urn:vision", name = "reason")
        val reason: List<Reason> = emptyList())

@XmlAccessorType(XmlAccessType.FIELD)
data class Reason(
        @XmlAttribute(name = "id")
        val id: String,
        @XmlElement(name = "description")
        val description: List<ReasonDescription> = emptyList(),
        @XmlAttribute(name = "default")
        val default: Boolean = false)

@XmlAccessorType(XmlAccessType.FIELD)
data class ReasonDescription(
        @XmlValue
        val description: String,
        @XmlAttribute(name = "language")
        val language: String = "en_UK"
)
