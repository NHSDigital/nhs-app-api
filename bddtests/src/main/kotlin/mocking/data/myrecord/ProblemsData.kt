package mocking.data.myrecord

import mocking.emis.problems.AssociatedText
import mocking.emis.problems.EffectiveDate
import mocking.emis.problems.Observation
import mocking.emis.problems.ProblemMedicalRecord
import mocking.emis.problems.ProblemResponse
import mocking.emis.problems.ProblemResponseModel
import org.joda.time.DateTime

object ProblemsData {
    private const val PROBLEMS_NUMBER = 3
    private const val DATE_FOR_PROBLEM_YEAR = 2018
    private const val DATE_FOR_PROBLEM_MONTH = 5
    private const val DATE_FOR_PROBLEM_DAY = 14

    fun getDefaultProblemModel() : ProblemResponseModel {
        return ProblemResponseModel(
                medicalRecord = ProblemMedicalRecord(
                        problems = mutableListOf()
                )
        )
    }

    fun getProblemsData(): ProblemResponseModel {
        val problems = mutableListOf<ProblemResponse>()

        val date = DateTime().withDate(DATE_FOR_PROBLEM_YEAR, DATE_FOR_PROBLEM_MONTH, DATE_FOR_PROBLEM_DAY)

        for (i in 1..PROBLEMS_NUMBER) {
            problems.add(ProblemResponse(
                    status = "Past",
                    significance="Minor",
                    problemEndDate="2018-05-15T09:52:44.927",
                    observation = Observation(
                            effectiveDate = EffectiveDate("YearMonthDay", date.plusDays(i).toString()),
                            term="Conjunctivitis",
                            associatedText = mutableListOf(
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

    fun getBadVisionProblemsDataWithNoProblemsData(): String {
        val badData = "<problems eventdate=\"2018-10-10T00:00:00\" " +
                "badData=\"Peanut allergy\"  subgroup_code=\"PastProblem\"/> " +
                "<problems eventdate=\"2018-10-10T00:00:00\" " +
                "badData=\"Broken leg\"  subgroup_code=\"CurrentProblem\"/> " +
                "<problems eventdate=\"2018-10-10T00:00:00\" " +
                "badData=\"Acne\"  subgroup_code=\"Random\"/>"
        val response = "<![BADDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        return response + badData + responseStringEnd
    }

    fun getEmisProblemRecordsWhereTheSecondRecordHasNoEffectiveDate(): ProblemResponseModel {
        val problemResponseModel = getProblemsData()
        problemResponseModel.medicalRecord.problems[1].observation.effectiveDate = null
        return problemResponseModel
    }
}
