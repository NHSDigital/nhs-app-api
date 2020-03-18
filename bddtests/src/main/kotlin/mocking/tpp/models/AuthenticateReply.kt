package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "AuthenticateReply")
@XmlAccessorType(XmlAccessType.FIELD)
data class AuthenticateReply(
        @XmlAttribute var patientId: String = "default patientId",
        @XmlAttribute var onlineUserId: String = "default onlineUserId",
        @XmlAttribute var uuid: String = "default uuid",
        @field:XmlElement(name = "User") var user: User = User(
                Person(
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
        ),
        @field:XmlElement(name= "Person") var person:  MutableCollection<Person> = mutableListOf(
                Person("default patientId",
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
)
