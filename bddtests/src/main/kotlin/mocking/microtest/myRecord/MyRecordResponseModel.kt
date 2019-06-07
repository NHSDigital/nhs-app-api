package mocking.microtest.myRecord

data class MyRecordResponseModel(
        var allergies: Allergies
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
