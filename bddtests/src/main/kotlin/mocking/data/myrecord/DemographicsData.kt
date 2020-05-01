package mocking.data.myrecord

import constants.DateTimeFormats
import constants.TppConstants
import mocking.emis.demographics.Address
import mocking.emis.demographics.ContactDetails
import mocking.emis.demographics.EmisDemographicsResponse
import mocking.emis.demographics.PatientIdentifier
import mocking.emis.models.IdentifierType
import mocking.tpp.models.NationalId
import mocking.tpp.models.PatientSelectedReply
import mocking.tpp.models.Person
import mocking.tpp.models.PersonName
import mocking.tpp.models.TppAddress
import models.Patient
import utils.DateConverter


object DemographicsData {

    fun getEmisDemographicData(patient: Patient): EmisDemographicsResponse {
        val patientIdentifiers = mutableListOf<PatientIdentifier>()

        patient.nhsNumbers.forEach {
            patientIdentifiers.add(PatientIdentifier(it, IdentifierType.NhsNumber))
        }

        return EmisDemographicsResponse(
                patient.name.title,
                patient.name.firstName,
                patient.name.surname,
                "Johnny",
                patientIdentifiers,
                DateConverter.convertDateToDateTimeFormat(
                        patient.age.dateOfBirth,
                        DateTimeFormats.dateWithoutTimeFormat,
                        DateTimeFormats.backendDateTimeFormatWithoutTimezone),
                patient.sex,
                ContactDetails(patient.contactDetails.telephoneFirst,
                        patient.contactDetails.telephoneSecond,
                        patient.contactDetails.emailAddress),
                Address(patient.contactDetails.address.houseNameFlatNumber,
                        patient.contactDetails.address.numberStreet,
                        patient.contactDetails.address.village,
                        patient.contactDetails.address.town,
                        patient.contactDetails.address.county,
                        patient.contactDetails.address.postcode)
        )
    }

    fun getTppDemographicsData(patient: Patient): PatientSelectedReply {
        return PatientSelectedReply(
                patient.patientId,
                patient.onlineUserId,
                "1f907c07-9063-4d3a-81d7-ee8c98c54f4a",
                Person(patient.patientId,
                        DateConverter.convertDateToDateTimeFormat(
                                patient.age.dateOfBirth,
                                DateTimeFormats.dateWithoutTimeFormat,
                                DateTimeFormats.tppDateTimeFormat),
                        patient.sex.name,
                        NationalId(type = TppConstants.NationalIdTypeNhs, value = patient.nhsNumbers.first()),
                        PersonName(patient.formattedFullName()),
                        TppAddress(patient.contactDetails.address.full())
        ))
    }
}
