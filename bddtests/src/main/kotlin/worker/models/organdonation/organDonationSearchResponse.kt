package worker.models.organdonation


data class OrganDonationSearchResponse(
        var identifier: String,
        var nhsNumber: String,
        var nameFull: String,
        var name: Name,
        var gender: String,
        var dateOfBirth: String,
        var addressFull: String,
        var address: Address,
        var emailAddress: String,
        var decision: String,
        var decisionDetails: DecisionDetails,
        var faithDeclaration: String,
        var state: OrganDonationState
)

data class Name(var title: String,
                var givenName: String,
                var surname: String)

data class Address(var text: String,
                   var postCode: String)

data class DecisionDetails(var all: Boolean,
                           var choices: HashMap<String, String>)

enum class OrganDonationState{
    NotFound,
    Ok,
    Conflicted
}
