package mocking.emis.problems

import jxl.write.DateTime
import java.time.OffsetDateTime

data class ProblemResponseModel(
        var medicalRecord: ProblemMedicalRecord
)

data class ProblemMedicalRecord(
        var problems: MutableList<ProblemResponse> = arrayListOf()
)

data class ProblemResponse(
        var status: String,
        var significance: String,
        var problemEndDate: String,
        var observation: Observation
)

data class Observation(
        var term: String,
        var effectiveDate: EffectiveDate,
        var associatedText: MutableList<AssociatedText> = arrayListOf()
)

data class EffectiveDate(
        var datePart: String,
        var value: String
)

data class AssociatedText(
        var text: String
)

