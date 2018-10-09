package mocking.vision.Demographics

import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "demographics", namespace = "urn:vision")
data class Demographics(
        @XmlElement(namespace = "urn:vision", name="name")
        var name: Name? = null,
        @XmlElement(namespace = "urn:vision", name="maritalStatus")
        var maritalStatus: String? = null,
        @XmlElement(namespace = "urn:vision", name="dateOfBirth")
        var dateOfBirth: String? = null,
        @XmlElement(namespace = "urn:vision", name="gender")
        var gender: String? = null,
        @XmlElement(namespace = "urn:vision", name="primaryAddress")
        var primaryAddress: PrimaryAddress? = null,
        @XmlElement(namespace = "urn:vision", name="usualGP")
        var usualGP: String? = null,
        @XmlElement(namespace = "urn:vision", name="surgeryAttended")
        var surgeryAttended: String? = null)