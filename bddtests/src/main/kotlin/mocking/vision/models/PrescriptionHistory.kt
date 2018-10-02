package mocking.vision.models

import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "prescriptionHistory")
data class PrescriptionHistory(

    @XmlElement(namespace= "urn:vision", name = "request")
    var request: MutableList<Request> = mutableListOf()
)
