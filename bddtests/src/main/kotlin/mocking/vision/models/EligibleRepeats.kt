package mocking.vision.models

import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "eligibleRepeats")
data class EligibleRepeats(

        @XmlElement(namespace= "urn:vision", name = "settings")
        var settings: Settings = Settings(true),

        @XmlElement(namespace= "urn:vision", name = "repeat")
        var repeat: MutableList<RepeatCourse> = mutableListOf()

)
