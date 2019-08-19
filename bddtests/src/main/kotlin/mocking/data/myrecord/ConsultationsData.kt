package mocking.data.myrecord

import mocking.emis.consultations.AssociatedText
import mocking.emis.consultations.ConsultationMedicalRecord
import mocking.emis.consultations.ConsultationResponse
import mocking.emis.consultations.ConsultationsResponseModel
import mocking.emis.consultations.EffectiveDate
import mocking.emis.consultations.Observation
import mocking.emis.consultations.Section

object ConsultationsData {

    private const val NUMBER_OF_CONSULTATION_RECORDS = 2

    fun getDefaultConsultationsData() : ConsultationsResponseModel {
        return ConsultationsResponseModel(
                medicalRecord = ConsultationMedicalRecord(
                        consultations = mutableListOf()
                )
        )
    }

    fun getTwoConsultationsWhereTheSecondRecordHasNoDate() : ConsultationsResponseModel {
        val consultationsResponseModel = getMultipleConsultationRecords(2)
        consultationsResponseModel.medicalRecord.consultations[1].effectiveDate.value = ""

        return consultationsResponseModel;
    }

    fun getMultipleConsultationRecords(count :Int = NUMBER_OF_CONSULTATION_RECORDS): ConsultationsResponseModel {

        val consultations = mutableListOf<ConsultationResponse>()

        for(i in 1..count){
            consultations.add(ConsultationResponse("THE SURGERY - MOSS", "Jean (Dr)",
                    mutableListOf(Section("History", mutableListOf(
                            Observation("C/O: a rash", mutableListOf(
                                    AssociatedText("Tired generally. Needs to have bloods etc")))))),
                    EffectiveDate("YearMonthDay", "2018-02-18T14:23:44.927")))
        }

        return ConsultationsResponseModel(
                medicalRecord = ConsultationMedicalRecord(
                        consultations = consultations
                )
        )
    }
}
