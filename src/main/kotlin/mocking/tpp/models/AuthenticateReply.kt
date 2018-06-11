package mocking.tpp.models

import javax.xml.bind.annotation.*

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
        )
)