package mocking.vision.models

import org.joda.time.DateTime
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "settings")
data class Settings(
                   @XmlElement(namespace= "urn:vision",name = "allowFreeText")
                   var allowFreeText: Boolean?
) {
    constructor() : this( null)
}
