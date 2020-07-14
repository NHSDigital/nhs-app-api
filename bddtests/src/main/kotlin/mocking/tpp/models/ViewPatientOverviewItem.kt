package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement
import javax.xml.bind.annotation.XmlValue

@XmlRootElement(name = "Item")
@XmlAccessorType(XmlAccessType.FIELD)
data class ViewPatientOverviewItem(
        @XmlAttribute var id: String = "default patientId",
        @XmlAttribute var description: String = "default description",
        @XmlAttribute var date: String = "default date",
        @XmlValue var value: String = "default value"
)
