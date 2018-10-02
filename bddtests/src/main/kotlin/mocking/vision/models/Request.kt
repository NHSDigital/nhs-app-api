package mocking.vision.models

import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "request")
data class Request(

        @XmlElement(namespace= "urn:vision", name = "date")
        var date: String?,

        @XmlElement(namespace = "urn:vision", name = "status")
        var status: StatusCode?,

        @XmlElement(namespace= "urn:vision", name = "repeat")
        var repeat: ArrayList<Repeat> = arrayListOf()) {

                constructor() : this(null, null, arrayListOf())
        }


