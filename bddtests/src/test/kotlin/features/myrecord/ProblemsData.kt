package features.myrecord

import mocking.emis.problems.*

object ProblemsData {

    fun getDefaultProblemModel(): ProblemResponseModel {
        return ProblemResponseModel(
                medicalRecord = ProblemMedicalRecord(
                        problems = mutableListOf()
                )
        )
    }

    fun getProblemsData(): ProblemResponseModel {

        val problems = mutableListOf<ProblemResponse>()

        for (i in 1..3) {
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
                    )));
        }

        return ProblemResponseModel(
                medicalRecord = ProblemMedicalRecord(
                        problems = problems
                )
        )
    }

    fun getProblemRecordsWithDifferentDateParts(): ProblemResponseModel {

        val problems = mutableListOf<ProblemResponse>()

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
                )));
        problems.add(ProblemResponse(
                status = "Past",
                significance="Minor",
                problemEndDate="2018-05-15T09:52:44.927",
                observation = Observation(
                        effectiveDate = EffectiveDate("Year", "2018-05-15T09:52:44.927"),
                        term="Lower back pain",
                        associatedText = mutableListOf<AssociatedText> (
                                AssociatedText (text = "Patient advice given"),
                                AssociatedText (text = "Bend legs when lifting")
                        )
                )));
        problems.add(ProblemResponse(
                status = "Past",
                significance="Minor",
                problemEndDate="2018-05-15T09:52:44.927",
                observation = Observation(
                        effectiveDate = EffectiveDate("YearMonthDayTime", "2018-05-15T09:52:44.927"),
                        term="Sprained ankle",
                        associatedText = mutableListOf<AssociatedText> (
                                AssociatedText (text = "Patient told to wear ankle support")
                        )
                )));

        return ProblemResponseModel(
                medicalRecord = ProblemMedicalRecord(
                        problems = problems
                )
        )
    }
}
