package features.myrecord

import mocking.emis.immunisations.EffectiveDate
import mocking.emis.immunisations.ImmunisationMedicalRecord
import mocking.emis.immunisations.ImmunisationResponse
import mocking.emis.immunisations.ImmunisationResponseModel

object ImmunisationsData {

    fun getImmunisationsData(): ImmunisationResponseModel {

        val immunisations = mutableListOf<ImmunisationResponse>()

        immunisations.add(ImmunisationResponse(term = "Hay Fever",
                effectiveDate =  EffectiveDate("Unknown", "2018-05-15T09:52:44.927")))

        immunisations.add(ImmunisationResponse(term = "Hay Fever",
                effectiveDate =  EffectiveDate("YearMonthDay", "2019-02-18T14:23:44.927")))

        return ImmunisationResponseModel (
                medicalRecord = ImmunisationMedicalRecord (
                        immunisations = immunisations
                )
        )
    }
}