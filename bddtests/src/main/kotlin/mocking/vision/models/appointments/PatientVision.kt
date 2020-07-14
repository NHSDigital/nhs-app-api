package mocking.vision.models.appointments

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlElement

@XmlAccessorType(XmlAccessType.FIELD)
data class PatientVision(
        @XmlElement val id: String,
        @XmlElement val name: String? = null)
