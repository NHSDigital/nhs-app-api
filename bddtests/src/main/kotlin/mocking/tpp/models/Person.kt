package mocking.tpp.models

import javax.xml.bind.annotation.*

@XmlAccessorType(XmlAccessType.FIELD)
data class Person(
        @XmlAttribute var patientId: String = "default patientId",
        @XmlAttribute var dateOfBirth: String = "default dateOfBirth",
        @XmlAttribute var gender: String = "default gender",
        @field:XmlElement(name = "NationalId") var nationalId: NationalId = NationalId(
                "default type",
                "default value"
        ),
        @field:XmlElement(name = "PersonName") var personName: PersonName = PersonName(
                "default name"
        ),
        @field:XmlElement(name = "Address") var address: TppAddress = TppAddress(
        "default address"
)
)