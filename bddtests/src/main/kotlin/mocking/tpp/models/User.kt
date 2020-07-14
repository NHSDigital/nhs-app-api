package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlElement

@XmlAccessorType(XmlAccessType.FIELD)
data class User(
        @field:XmlElement(name = "Person") var person: Person = Person(
                "default patientId",
                "default dateOfBirth",
                "default gender",
                NationalId(
                        "default type",
                        "default value"
                ),
                PersonName(
                        "default name"
                )
        )
)
