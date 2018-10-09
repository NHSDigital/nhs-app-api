package mocking.vision.Demographics

import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name="primaryAddress", namespace = "urn:vision")
data class PrimaryAddress(
    @XmlElement(namespace="", name="houseName")
    var houseName: String? = null,
    @XmlElement(namespace="", name="houseNumber")
    var houseNumber: String? = null,
    @XmlElement(namespace="", name="street")
    var street: String? = null,
    @XmlElement(namespace="", name="town")
    var town: String? = null,
    @XmlElement(namespace="", name="county")
    var county: String? = null,
    @XmlElement(namespace="", name="postcode")
    var postcode: String? = null
)
{
    constructor() : this("", "", "", "")
}