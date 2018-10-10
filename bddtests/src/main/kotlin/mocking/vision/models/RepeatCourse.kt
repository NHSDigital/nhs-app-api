package mocking.vision.models

import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "repeat")
data class RepeatCourse(
        @XmlAttribute(name = "id") private var id: String?,

        @XmlElement(namespace= "urn:vision",name = "drug")
        var drug: String?,

        @XmlElement(namespace = "urn:vision", name = "dosage")
        var dosage: String?,

        @XmlElement(namespace= "urn:vision",name = "quantity")
        var quantity: String?,

        @XmlElement(namespace= "urn:vision",name = "lastIssued")
        var lastIssued: String?

) {
    constructor() : this( null, null, null, null, null)
}