package mocking.data.myrecord

import mocking.emis.consultations.Observation
import mocking.emis.consultations.AssociatedText
import mocking.emis.consultations.EffectiveDate
import mocking.emis.consultations.ConsultationMedicalRecord
import mocking.emis.consultations.ConsultationResponse
import mocking.emis.consultations.ConsultationsResponseModel
import mocking.emis.consultations.Section

object ConsultationsData {

    fun getDefaultConsultationsData() : ConsultationsResponseModel {
        return ConsultationsResponseModel(
                medicalRecord = ConsultationMedicalRecord(
                        consultations = mutableListOf()
                )
        )
    }

    fun getMultipleConsultationRecords(): ConsultationsResponseModel {

        val consultations = mutableListOf<ConsultationResponse>()

        consultations.add(ConsultationResponse("THE SURGERY - MOSS", "Jean (Dr)",
                mutableListOf(Section("History", mutableListOf(
                        Observation("C/O: a rash", mutableListOf(
                                AssociatedText("Tired generally. Needs to have bloods etc")))))),
                                EffectiveDate("YearMonthDay", "2018-02-18T14:23:44.927")))

        consultations.add(ConsultationResponse("THE SURGERY - LAMBERT", "James (Dr)",
                mutableListOf(Section("History", mutableListOf(
                        Observation("C/O: a lump", mutableListOf(
                                AssociatedText("Tired. Needs to have bloods etc")))))),
                EffectiveDate("YearMonthDay", "2016-02-22T19:34:44.327")))

        return ConsultationsResponseModel(
                medicalRecord = ConsultationMedicalRecord(
                        consultations = consultations
                )
        )
    }
}
