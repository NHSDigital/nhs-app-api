package mocking.vision.models.appointments

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement


@XmlRootElement(name = "appointment")
@XmlAccessorType(XmlAccessType.FIELD)
data class Appointment(
        @XmlElement
        val patient: PatientVision? = null,
        @XmlElement
        val slot: Slot? = null
)
