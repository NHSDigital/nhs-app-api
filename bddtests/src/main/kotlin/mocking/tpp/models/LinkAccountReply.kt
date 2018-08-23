package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "LinkAccountReply")
@XmlAccessorType(XmlAccessType.FIELD)
data class LinkAccountReply (
        @XmlAttribute var passphrase: String = "default passphrase",
        @XmlAttribute var uuid: String = "default uuid"
)
