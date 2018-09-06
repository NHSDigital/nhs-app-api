package mocking.data.myrecord

import constants.AppointmentDateTimeFormat
import constants.TppConstants
import mocking.emis.demographics.Address
import mocking.emis.demographics.ContactDetails
import mocking.emis.demographics.EmisDemographicsResponse
import mocking.emis.demographics.PatientIdentifier
import mocking.emis.demographics.Sex
import mocking.emis.models.IdentifierType
import mocking.tpp.models.NationalId
import mocking.tpp.models.PatientSelectedReply
import mocking.tpp.models.Person
import mocking.tpp.models.PersonName
import mocking.tpp.models.TppAddress
import models.Patient
import utils.DateConverter


object DemographicsData {

    fun getEmisDemographicData(patient: Patient) : EmisDemographicsResponse {
        val patientIdentifiers = mutableListOf<PatientIdentifier>()

        patient.nhsNumbers.forEach {
            patientIdentifiers.add(PatientIdentifier(it, IdentifierType.NhsNumber))
        }

        return EmisDemographicsResponse(
                patient.title,
                patient.firstName,
                patient.surname,
                "Johnny",
                patientIdentifiers,
                DateConverter.ConvertDateToDateTimeFormat(patient.dateOfBirth, AppointmentDateTimeFormat.mockDataDobFormat, AppointmentDateTimeFormat.backendDateTimeFormatWithoutTimezone),
                patient.sex,
                ContactDetails(patient.contactDetails.telephoneNumber, patient.contactDetails.mobileNumber, patient.contactDetails.emailAddress),
                Address(patient.address.houseNameFlatNumber,
                        patient.address.numberStreet,
                        patient.address.village,
                        patient.address.town,
                        patient.address.county,
                        patient.address.postcode)
        )
    }

    fun getTppDemographicsData(patient: Patient): PatientSelectedReply {
        return PatientSelectedReply(
                patient.patientId,
                patient.onlineUserId,
                "1f907c07-9063-4d3a-81d7-ee8c98c54f4a",
                Person(patient.patientId,
                        DateConverter.ConvertDateToDateTimeFormat(patient.dateOfBirth, AppointmentDateTimeFormat.mockDataDobFormat, AppointmentDateTimeFormat.backendDateTimeFormat),
                        patient.sex.name,
                        NationalId(type = TppConstants.NationalIdTypeNhs, value = patient.nhsNumbers.first()),
                        PersonName(patient.title + " " + patient.firstName + " " + patient.surname),
                        TppAddress("${patient.address.houseNameFlatNumber}, ${patient.address.numberStreet}, ${patient.address.village}, ${patient.address.town}, ${patient.address.county}, ${patient.address.postcode}"))
        )
    }
}
