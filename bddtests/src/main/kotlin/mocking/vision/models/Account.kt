package mocking.vision.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement
import javax.xml.bind.annotation.XmlElement

data class Account(@XmlElement(namespace = "urn:vision")
                   var patientId: String,

                   @XmlElement(namespace= "urn:vision",name = "patientNumber")
                   var patientNumber: List<PatientNumber>?,

                   @XmlElement(namespace = "urn:vision")
                   var name: String) {
    constructor() : this("", null, "")
}
