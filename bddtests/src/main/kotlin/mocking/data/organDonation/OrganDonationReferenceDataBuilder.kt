package mocking.data.organDonation

import mocking.organDonation.models.Coding
import mocking.organDonation.models.Entry
import mocking.organDonation.models.OrganDonationSuccessResponse
import mocking.organDonation.models.ReferenceDataResponse
import utils.set

class OrganDonationReferenceDataBuilder {

    companion object {

        val hindhu = Coding(code = "25", display = "Hindhu")
        val chinese = Coding(code = "15", display = "Chinese")

        fun build(): OrganDonationSuccessResponse<ReferenceDataResponse> {

            val religionsResource = ReferenceDataResponse(
                    "religions",
                    arrayListOf(
                            Coding(code = "01", display = "No religion"),
                            Coding(code = "02", display = "Christian - Protestant"),
                            Coding(code = "10", display = "Christian - Catholic"),
                            Coding(code = "19", display = "Christian - other"),
                            Coding(code = "20", display = "Buddhist"),
                            hindhu,
                            Coding(code = "30", display = "Jewish"),
                            Coding(code = "35", display = "Muslim"),
                            Coding(code = "40", display = "Sikh"),
                            Coding(code = "60", display = "Other"),
                            Coding(code = "88", display = "Not stated")
                    ))

            OrganDonationSerenityHelpers.REFERENCE_RELIGIONS
                    .set(religionsResource.concept.map { concept -> concept.display })

            val ethnicitiesResource = ReferenceDataResponse(
                    "ethnicities",
                    arrayListOf(
                            Coding(code = "1", display = "White - British"),
                            Coding(code = "2", display = "White - Irish"),
                            Coding(code = "3", display = "White - Any Other White Background"),
                            Coding(code = "4", display = "Mixed - White and Black Caribbean"),
                            Coding(code = "5", display = "Mixed - White and Black African"),
                            Coding(code = "6", display = "Mixed - White and Asian"),
                            Coding(code = "7", display = "Mixed - Any Other Mixed Background"),
                            Coding(code = "8", display = "Asian - Indian"),
                            Coding(code = "9", display = "Asian - Pakistani"),
                            Coding(code = "10", display = "Asian - Bangladeshi"),
                            Coding(code = "11", display = "Asian - Any Other Asian Background"),
                            Coding(code = "12", display = "Black - Caribbean"),
                            Coding(code = "13", display = "Black - African"),
                            Coding(code = "14", display = "Black - Any Other Black Background"),
                            chinese,
                            Coding(code = "16", display = "Other - Any Other Ethnic Category"),
                            Coding(code = "17", display = "Gypsy or Irish Traveller"),
                            Coding(code = "18", display = "Arab"),
                            Coding(code = "77", display = "Not Stated"),
                            Coding(code = "88", display = "Not Reported"))
            )

            OrganDonationSerenityHelpers.REFERENCE_ETHNICITIES
                    .set(ethnicitiesResource.concept.map { concept -> concept.display })

            val withdrawalReasonsResource = ReferenceDataResponse(
                    "withdraw-reasons",
                    arrayListOf(
                            Coding(code = "1", display = "Request for Withdrawal"),
                            Coding(code = "2", display = "Withdrawn As Leaving UK"),
                            Coding(code = "3", display = "Religious Grounds"),
                            Coding(code = "4", display = "Medical Condition"),
                            Coding(code = "5", display = "Registered In Error"),
                            Coding(code = "6", display = "Change of Mind"),
                            Coding(code = "7", display = "Family Do Not Agree"),
                            Coding(code = "8", display = "Other")
                    )
            )

            OrganDonationSerenityHelpers.REFERENCE_WITHDRAWAL_REASONS
                    .set(withdrawalReasonsResource.concept.map { concept -> concept.display })

            return OrganDonationSuccessResponse(
                    listOf(Entry(religionsResource), Entry(ethnicitiesResource), Entry(withdrawalReasonsResource)))
        }
    }
}
