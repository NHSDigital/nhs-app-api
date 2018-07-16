package mocking.vision.models

import javax.xml.bind.annotation.*

@XmlRootElement(name = "configuration")
data class Configuration(@XmlElement(namespace = "urn:vision")
                         var account: Account?) {
    constructor() : this(null)
}
