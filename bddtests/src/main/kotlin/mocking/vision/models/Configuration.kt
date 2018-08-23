package mocking.vision.models

import javax.xml.bind.annotation.XmlRootElement
import javax.xml.bind.annotation.XmlElement

@XmlRootElement(name = "configuration")
data class Configuration(@XmlElement(namespace = "urn:vision")
                         var account: Account?) {
    constructor() : this(null)
}
