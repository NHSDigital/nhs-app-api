package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "Appointment")
@XmlAccessorType(XmlAccessType.FIELD)
data class Appointment(
    @XmlAttribute var apptId: String = "default apptId",
    @XmlAttribute var startDate: String = "default startDate",
    @XmlAttribute var endDate: String = "default endDate",
    @XmlAttribute var details: String = "default details",
    @XmlAttribute var siteName: String = "default siteName",
    @XmlAttribute var address: String = "default address"
)
