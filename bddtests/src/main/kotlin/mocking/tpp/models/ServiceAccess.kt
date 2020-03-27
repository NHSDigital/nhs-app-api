package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "ServiceAccess")
@XmlAccessorType(XmlAccessType.FIELD)
data class ServiceAccess(
        @XmlAttribute
        var description: String = "default description",
        @XmlAttribute
        var serviceIdentifier: String = "default serviceIdentifier",
        @XmlAttribute
        var status: String = "default status",
        @XmlAttribute
        var statusDesc: String = "default statusDesc",
        @XmlAttribute
        var readOnly: String = "default readOnly"
)