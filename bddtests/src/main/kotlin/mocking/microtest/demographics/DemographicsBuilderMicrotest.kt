package mocking.microtest.demographics

import mocking.GsonFactory
import mocking.microtest.MicrotestMappingBuilder
import mocking.models.Mapping
import models.Patient
import org.apache.http.HttpStatus

class DemographicsBuilderMicrotest(private val patient: Patient)
    : MicrotestMappingBuilder(method = "GET", relativePath = "/demographics") {

    fun respondWithSuccess(): Mapping {
        val response = DemographicsData(
                title = patient.name.title,
                forenames1 = patient.name.firstName,
                surname = patient.name.surname,
                dob = patient.age.dateOfBirth,
                sex = patient.sex.toString(),
                nhs = patient.formattedNHSNumber(),
                houseName = patient.contactDetails.address.houseNameFlatNumber!!,
                roadName = patient.contactDetails.address.numberStreet!!,
                locality = patient.contactDetails.address.village!!,
                post_town = patient.contactDetails.address.town!!,
                county = patient.contactDetails.address.county!!,
                postcode = patient.contactDetails.address.postcode!!,
                telephone1 = patient.contactDetails.telephoneFirst,
                telephone2 = patient.contactDetails.telephoneSecond
                )
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(GetDemographicsResponseModel(response), GsonFactory.asIs)
        }
    }
}
