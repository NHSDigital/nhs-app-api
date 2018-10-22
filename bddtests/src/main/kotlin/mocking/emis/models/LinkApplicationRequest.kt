package mocking.emis.models

import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "LinkApplicationRequest")
data class LinkApplicationRequest(
        @field:XmlElement(name = "Surname") var surname: String,
        @field:XmlElement(name = "DateOfBirth") var dateOfBirth: String) {

    @field:XmlElement(name = "LinkageDetails")
    var linkageDetails: LinkageDetails? = null

    constructor(surname: String, dateOfBirth: String,
                accountId: String, linkageKey: String,
                nationalPracticeCode: String) : this(surname, dateOfBirth) {
        linkageDetails = LinkageDetails(accountId, linkageKey, nationalPracticeCode)
    }
}