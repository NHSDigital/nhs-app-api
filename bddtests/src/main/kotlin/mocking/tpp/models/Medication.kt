package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "Medication")
@XmlAccessorType(XmlAccessType.FIELD)
data class Medication(
        @XmlAttribute var drugId: String = "default drugId",
        @XmlAttribute var type: String = "default type",
        @XmlAttribute var drug: String = "default drug",
        @XmlAttribute var details: String = "default details",
        @XmlAttribute var requestable: String = "default requestable"
)
