package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement

@XmlAccessorType(XmlAccessType.FIELD)
data class SiteDetails(
        @XmlAttribute var unitName: String = "default unit name",
        @field:XmlElement(name = "Address") var address: TppAddress = TppAddress()
)
