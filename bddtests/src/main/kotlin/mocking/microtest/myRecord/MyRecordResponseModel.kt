package mocking.microtest.myRecord

data class MyRecordResponseModel(
        var allergies: Allergies,
        var drugs: Medications,
        var vaccinations: Immunisations,
        var medicalProblems: Problems,
        var testResults: TestResults,
        var medicalHistory: MedicalHistories,
        var recalls: Recalls,
        var encounters: Encounters,
        var referral: Referrals
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

data class Encounter(
        var recordedOn: String,
        var description: String,
        var value: String,
        var unit: String
)

data class Encounters(
        var hasAccess: String,
        var hasErrored: String,
        var count: Int,
        var data: MutableList<Encounter> = arrayListOf()
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

data class Immunisations(
        var hasAccess: String,
        var hasErrored: String,
        var count: Int,
        var data: MutableList<Immunisation> = arrayListOf()
)

data class Immunisation(
        var date: String,
        var description: String,
        var nextDate: String,
        var status: String
)

data class Problems(
        var hasAccess: String,
        var hasErrored: String,
        var count: Int,
        var data: MutableList<Problem> = arrayListOf()
)

data class Problem(
        var start_date: String,
        var finish_date: String,
        var rubric: String
)

data class Referral(
        var recordDate: String,
        var description: String,
        var speciality: String,
        var ubrn: String
)

data class Referrals(
        var hasAccess: String,
        var hasErrored: String,
        var count: Int,
        var data: MutableList<Referral> = arrayListOf()
)

data class TestResults(
        var hasAccess: String,
        var hasErrored: String,
        var count: Int,
        var data: TestResultData
)

data class TestResultData(
        var inrResults: InrResults,
        var pathResults: PathResults
)

data class InrResults(
        var count: Int,
        var data: MutableList<InrResult> = arrayListOf()
)

data class PathResults(
        var count: Int,
        var data: MutableList<PathResult> = arrayListOf()
)

data class InrResult(
        var recordDateTime: String,
        var codeDescr: String,
        var therapy: String,
        var target: String,
        var value: String,
        var dose: String,
        var nextTestDate: String
)

data class PathResult(
        var name: String,
        var recordDate: String,
        var elementName: String,
        var value: String,
        var units: String,
        var status: String
)

data class MedicalHistories(
        var hasAccess: String,
        var hasErrored: String,
        var count: Int,
        var data: MutableList<MedicalHistory> = arrayListOf()
)

data class MedicalHistory(
        var start_date: String,
        var rubric: String,
        var description: String
)

data class Recalls(
        var hasAccess: String,
        var hasErrored: String,
        var count: Int,
        var data: MutableList<Recall> = arrayListOf()
)

data class Recall(
        var recordDate: String,
        var name: String,
        var description: String,
        var result: String,
        var nextDate: String,
        var status: String
)
