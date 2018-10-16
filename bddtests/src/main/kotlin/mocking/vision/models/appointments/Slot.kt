package mocking.vision.models.appointments

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement

@XmlAccessorType(XmlAccessType.FIELD)
data class Slot(
        @XmlAttribute(name = "id") private var id: String? = null,
        @XmlElement(namespace = "urn:vision")
        var dateTime: String? = null,
        @XmlElement(namespace = "urn:vision")
        var owner: String? = null,
        @XmlElement(namespace = "urn:vision")
        var location: Int? = null,
        @XmlElement(namespace = "urn:vision")
        var type: String? = null,
        @XmlElement(namespace = "urn:vision")
        var session: Int? = null
)
