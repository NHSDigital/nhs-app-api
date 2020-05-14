package mocking.vision.models.appointments

import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement

@Suppress("UnusedPrivateMember", "Required for xml serialisation")
data class Session(
        @XmlAttribute(name = "id") private var id: Int?,
        @XmlElement(namespace = "urn:vision")
        var description: String
)
