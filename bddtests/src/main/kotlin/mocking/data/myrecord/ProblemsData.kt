package mocking.data.myrecord

import mocking.emis.problems.AssociatedText
import mocking.emis.problems.EffectiveDate
import mocking.emis.problems.Observation
import mocking.emis.problems.ProblemMedicalRecord
import mocking.emis.problems.ProblemResponse
import mocking.emis.problems.ProblemResponseModel

object ProblemsData {
    private const val PROBLEMS_NUMBER = 3

    fun getDefaultProblemModel() : ProblemResponseModel {
        return ProblemResponseModel(
                medicalRecord = ProblemMedicalRecord(
                        problems = mutableListOf()
                )
        )
    }

    fun getProblemsData(): ProblemResponseModel {
        val problems = mutableListOf<ProblemResponse>()

        for (i in 1..PROBLEMS_NUMBER) {
            problems.add(ProblemResponse(
                    status = "Past",
                    significance="Minor",
                    problemEndDate="2018-05-15T09:52:44.927",
                    observation = Observation(
                            effectiveDate = EffectiveDate("UnKnown", "2018-05-15T09:52:44.927"),
                            term="Conjunctivitis",
                            associatedText = mutableListOf<AssociatedText> (
                                    AssociatedText (text = "Patient advice given"),
                                    AssociatedText (text = "Repeated use of eye drops")
                            )
                    )))
        }

        return ProblemResponseModel(
                medicalRecord = ProblemMedicalRecord(
                        problems = problems
                )
        )
    }

    fun getVisionProblemsData(): String {
        val problem =
                "<problems eventdate=\"2018-10-10T00:00:00\" " +
                "read_term=\"Peanut allergy\"  subgroup_code=\"PastProblem\"/> " +
                "<problems eventdate=\"2018-10-10T00:00:00\" " +
                "read_term=\"Broken leg\"  subgroup_code=\"CurrentProblem\"/> " +
                "<problems eventdate=\"2018-10-10T00:00:00\" " +
                "read_term=\"Acne\"  subgroup_code=\"Random\"/>"

        val response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        return response + problem + responseStringEnd
    }

    fun getVisionProblemsDataWithNoProblemsData(): String {
        val response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        return response + responseStringEnd
    }
}
