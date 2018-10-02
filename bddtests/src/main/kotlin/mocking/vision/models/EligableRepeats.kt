package mocking.vision.models

import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "eligableRepeats")
data class EligableRepeats(
                   @XmlElement(namespace= "urn:vision",name = "settings")
                   var settings: Settings?,
                   @XmlElement(namespace= "urn:vision",name = "repeat")
                   var request: MutableList<RepeatCourse>?){
    constructor() : this( null, null)
}
