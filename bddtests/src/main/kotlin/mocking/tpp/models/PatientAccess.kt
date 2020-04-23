package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement

@XmlAccessorType(XmlAccessType.FIELD)
data class PatientAccess(
        @XmlAttribute var patientId: String = "",
        @field:XmlElement(name = "SiteDetails") var siteDetails: SiteDetails = SiteDetails()
)
