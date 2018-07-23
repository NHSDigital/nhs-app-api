package mocking.tpp.models

import javax.xml.bind.annotation.*

@XmlRootElement(name = "LogOffReply")
@XmlAccessorType(XmlAccessType.FIELD)
data class LogOffReply(
        @XmlAttribute var uuid: String = "default uuid"
)