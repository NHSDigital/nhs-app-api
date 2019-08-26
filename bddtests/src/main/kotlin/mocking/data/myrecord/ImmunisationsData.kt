package mocking.data.myrecord

import mocking.emis.immunisations.EffectiveDate
import mocking.emis.immunisations.ImmunisationMedicalRecord
import mocking.emis.immunisations.ImmunisationResponse
import mocking.emis.immunisations.ImmunisationResponseModel

const val NUMBER_OF_IMMUNISATION_RECORDS = 2

object ImmunisationsData {

    fun getImmunisationsData(): ImmunisationResponseModel {

        val immunisations = mutableListOf<ImmunisationResponse>()

        immunisations.add(ImmunisationResponse(term = "First meningitis C Vaccination",
                effectiveDate =  EffectiveDate("Unknown", "2002-05-15T09:52:44.927")))

        immunisations.add(ImmunisationResponse(term = "Second meningitis C Vaccination",
                effectiveDate =  EffectiveDate("YearMonthDay", "2018-02-18T14:23:44.927")))

        return ImmunisationResponseModel (
                medicalRecord = ImmunisationMedicalRecord (
                        immunisations = immunisations
                )
        )
    }

    fun getValidImmunisationsData(): ImmunisationResponseModel {

        val immunisations = mutableListOf<ImmunisationResponse>()

        immunisations.add(ImmunisationResponse(term = "First meningitis C Vaccination",
                effectiveDate =  EffectiveDate("YearMonthDay", "2002-05-15T09:52:44.927")))

        immunisations.add(ImmunisationResponse(term = "Second meningitis C Vaccination",
                effectiveDate =  EffectiveDate("YearMonthDay", "2018-02-18T14:23:44.927")))

        return ImmunisationResponseModel (
                medicalRecord = ImmunisationMedicalRecord (
                        immunisations = immunisations
                )
        )
    }

    fun getDefaultImmunisationsModel(): ImmunisationResponseModel {

        return ImmunisationResponseModel (
                medicalRecord = ImmunisationMedicalRecord (
                        immunisations = mutableListOf()
                )
        )
    }

    fun getVisionImmunisationsData(count: Int): String {
        val immunisation = "<clinical eventdate=\"2018-10-10T00:00:00\" read_term=\"Lumpectomy NEC\"" +
                " subgroup_code=\"Immunisation\"/>"

        var response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        for(i in 1..count) {
            response += immunisation
        }

        return response + responseStringEnd
    }

    fun getVisionImmunisationsDataWithNoImmunisations(): String {
        val response = "<![CDATA[<root><patient>"
        val responseStringEnd = "</patient></root>]]>"

        return response + responseStringEnd
    }

    fun getTwoImmunisationResultsWhereTheFirstRecordHasNoDate(): ImmunisationResponseModel {
        val immunisationData = getValidImmunisationsData()
        // Overwrite effective date
        immunisationData.medicalRecord.immunisations.first().effectiveDate.value = ""
        return immunisationData
    }

}
