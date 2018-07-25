package mocking.data.myrecord

import mocking.emis.immunisations.EffectiveDate
import mocking.emis.immunisations.ImmunisationMedicalRecord
import mocking.emis.immunisations.ImmunisationResponse
import mocking.emis.immunisations.ImmunisationResponseModel

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

    fun getDefaultImmunisationsModel(): ImmunisationResponseModel {

        return ImmunisationResponseModel (
                medicalRecord = ImmunisationMedicalRecord (
                        immunisations = mutableListOf()
                )
        )
    }
}
