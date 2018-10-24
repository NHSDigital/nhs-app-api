package mocking.vision.demographics

import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name="name", namespace = "urn:vision")
data class Name(@XmlElement(namespace = "urn:vision", name = "title")
                var title: String,
                @XmlElement(namespace= "urn:vision",name = "forename")
                var forename: String,
                @XmlElement(namespace= "urn:vision",name = "surname")
                var surname: String)
{
    constructor() : this("", "", "")
}
