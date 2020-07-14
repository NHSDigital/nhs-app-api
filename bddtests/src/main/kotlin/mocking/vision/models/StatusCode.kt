package mocking.vision.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement
import javax.xml.bind.annotation.XmlValue

@XmlAccessorType(XmlAccessType.FIELD)
@XmlRootElement(name = "status")
data class StatusCode(

    @XmlAttribute(name = "code")
    var code: Int,

    @XmlValue
    var statusValue: String
)
{
    constructor(): this(PrescriptionRepeatStatusCode.None, "")
}
