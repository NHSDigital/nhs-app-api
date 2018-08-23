package mocking.vision.models

import javax.xml.bind.annotation.XmlElement

data class PatientNumber(@XmlElement(namespace = "urn:vision")
                         var numberType: String = "NHS",

                         @XmlElement(namespace = "urn:vision")
                         var number: String) {

    constructor() : this("NHS", "")

}
