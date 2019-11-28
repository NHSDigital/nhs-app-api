package mocking.data.myrecord

import mocking.emis.consultations.AssociatedText
import mocking.emis.consultations.ConsultationMedicalRecord
import mocking.emis.consultations.ConsultationResponse
import mocking.emis.consultations.ConsultationsResponseModel
import mocking.emis.consultations.EffectiveDate
import mocking.emis.consultations.Observation
import mocking.emis.consultations.Section
import org.joda.time.DateTime

object ConsultationsData {

    private const val NUMBER_OF_CONSULTATION_RECORDS = 2
    private const val DATE_FOR_CONSULTATION_YEAR = 2018
    private const val DATE_FOR_CONSULTATION_MONTH = 2
    private const val DATE_FOR_CONSULTATION_DAY = 16

    fun getDefaultConsultationsData() : ConsultationsResponseModel {
        return ConsultationsResponseModel(
                medicalRecord = ConsultationMedicalRecord(
                        consultations = mutableListOf()
                )
        )
    }

    fun getTwoConsultationsWhereTheFirstRecordHasNoDate() : ConsultationsResponseModel {
        val consultationsResponseModel = getMultipleConsultationRecords(2)
        consultationsResponseModel.medicalRecord.consultations[0].effectiveDate = null

        return consultationsResponseModel;
    }

    fun getMultipleConsultationRecords(count :Int = NUMBER_OF_CONSULTATION_RECORDS): ConsultationsResponseModel {

        val consultations = mutableListOf<ConsultationResponse>()

        val date = DateTime().withDate(DATE_FOR_CONSULTATION_YEAR, DATE_FOR_CONSULTATION_MONTH,
                DATE_FOR_CONSULTATION_DAY)

        for(i in 1..count){
            consultations.add(ConsultationResponse("THE SURGERY - MOSS", "Jean (Dr)",
                    mutableListOf(Section("History", mutableListOf(
                            Observation("C/O: a rash", mutableListOf(
                                    AssociatedText("Tired generally. Needs to have bloods etc")))))),
                    EffectiveDate("YearMonthDay", date.plusDays(i).toString())))
        }

        return ConsultationsResponseModel(
                medicalRecord = ConsultationMedicalRecord(
                        consultations = consultations
                )
        )
    }
}
