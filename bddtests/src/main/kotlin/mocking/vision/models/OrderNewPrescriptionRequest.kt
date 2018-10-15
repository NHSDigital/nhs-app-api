package mocking.vision.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlElement

@XmlAccessorType(XmlAccessType.FIELD)
data class OrderNewPrescriptionRequest(
        @XmlElement(namespace = "urn:vision")
        var patientId: String,

        @XmlElement(namespace = "urn:vision")
        var repeat: List<NewPrescriptionRepeat>,

        @XmlElement(namespace = "urn:vision")
        var message: String
)