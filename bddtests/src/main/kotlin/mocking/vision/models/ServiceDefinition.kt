package mocking.vision.models

import javax.xml.bind.annotation.*

data class ServiceDefinition(
        @XmlElement(namespace = "urn:vision")
        var name: String,
        @XmlElement(namespace = "urn:vision")
        var version: String
) {
    constructor() : this("", "")
}