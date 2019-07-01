package mocking.microtest.myRecord

data class MyRecordResponseModel(
        var allergies: Allergies,
        var drugs: Medications
)

data class Allergies(
        var hasAccess: String,
        var hasErrored: String,
        var count: Int,
        var data: MutableList<Allergy> = arrayListOf()
)

data class Allergy(
        var id: String,
        var type: String,
        var start_date: String,
        var description: String,
        var severity: String
)

data class Medications(
        var hasAccess: String,
        var hasErrored: String,
        var count: Int,
        var data: MutableList<Medication> = arrayListOf()
)

data class Medication(
        var id: String,
        var name: String,
        var quantity: String,
        var dosage: String,
        var status: String,
        var type: String,
        var prescribed_date: String,
        var first_prescribed_date: String,
        var reason: String
)
