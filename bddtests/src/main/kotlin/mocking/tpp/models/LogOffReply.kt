package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "LogOffReply")
@XmlAccessorType(XmlAccessType.FIELD)
data class LogOffReply(
        @XmlAttribute var uuid: String = "default uuid"
)
