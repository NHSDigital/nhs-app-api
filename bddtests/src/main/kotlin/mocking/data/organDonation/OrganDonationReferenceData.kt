package mocking.data.organDonation

import mocking.organDonation.models.Coding
import mocking.organDonation.models.Entry
import mocking.organDonation.models.OrganDonationSuccessResponse
import mocking.organDonation.models.ReferenceDataResponse
import net.serenitybdd.core.Serenity

object OrganDonationReferenceData {

    fun getOrganDonationReferenceData(): OrganDonationSuccessResponse<ReferenceDataResponse> {

        val religionsResource = ReferenceDataResponse(
                "religions",
                arrayListOf(
                        Coding(code = "01", display = "No religion"),
                        Coding(code = "02", display = "Christian - Protestant"),
                        Coding(code = "10", display = "Christian - Catholic"),
                        Coding(code = "19", display = "Christian - other"),
                        Coding(code = "20", display = "Buddhist"),
                        Coding(code = "25", display = "Hindhu"),
                        Coding(code = "30", display = "Jewish"),
                        Coding(code = "35", display = "Muslim"),
                        Coding(code = "40", display = "Sikh"),
                        Coding(code = "60", display = "Other"),
                        Coding(code = "88", display = "Not stated")
                ))

        Serenity.setSessionVariable("ReferenceReligions")
                .to(religionsResource.concept.map { concept -> concept.display })

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
                        Coding(code = "15", display = "Chinese"),
                        Coding(code = "16", display = "Other - Any Other Ethnic Category"),
                        Coding(code = "17", display = "Gypsy or Irish Traveller"),
                        Coding(code = "18", display = "Arab"),
                        Coding(code = "77", display = "Not Stated"),
                        Coding(code = "88", display = "Not Reported"))
        )

        Serenity.setSessionVariable("ReferenceEthnicities")
                .to(ethnicitiesResource.concept.map { concept -> concept.display })

        return OrganDonationSuccessResponse(
                listOf(Entry(religionsResource), Entry(ethnicitiesResource)))
    }
}
