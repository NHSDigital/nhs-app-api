package mocking.vision.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute


@XmlAccessorType(XmlAccessType.FIELD)
data class NewPrescriptionRepeat(

        @XmlAttribute
        var id: String

)
