package mocking.tpp.models

import javax.xml.bind.annotation.*

@XmlRootElement(name = "Item")
@XmlAccessorType(XmlAccessType.FIELD)
data class Item(
        @XmlAttribute var id: String = "default patientId",
        @XmlAttribute var description: String = "default description",
        @XmlAttribute var date: String = "default date",
        @XmlValue var value: String = "default value"
)