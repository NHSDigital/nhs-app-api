package mocking.vision.models

import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "settings")
data class Settings(
                   @XmlElement(namespace= "urn:vision",name = "allowFreetext")
                   var allowFreetext: Boolean?
) {
    constructor() : this( null)
}
