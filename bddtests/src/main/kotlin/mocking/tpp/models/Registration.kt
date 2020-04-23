package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlElement

@XmlAccessorType(XmlAccessType.FIELD)
data class Registration(
        @field:XmlElement(name = "PatientAccess") var patientAccess: MutableCollection<PatientAccess> = mutableListOf()
)
