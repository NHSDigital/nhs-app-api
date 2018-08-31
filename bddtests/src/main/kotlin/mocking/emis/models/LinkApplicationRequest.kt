package mocking.emis.models

class LinkApplicationRequest(var Surname: String, var DateOfBirth: String) {
    var LinkageDetails: LinkageDetails? = null

    constructor(surname: String, dateOfBirth: String,
                accountId: String, linkageKey: String,
                nationalPracticeCode: String) : this(surname, dateOfBirth) {
        LinkageDetails = LinkageDetails(accountId, linkageKey, nationalPracticeCode)
    }
}