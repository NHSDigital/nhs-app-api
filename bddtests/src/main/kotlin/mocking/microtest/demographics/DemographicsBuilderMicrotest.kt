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
                title = patient.title,
                forenames1 = patient.firstName,
                surname = patient.surname,
                dob = patient.dateOfBirth,
                sex = patient.sex.toString(),
                nhs = patient.formattedNHSNumber(),
                houseName = patient.address.houseNameFlatNumber!!,
                roadName = patient.address.numberStreet!!,
                locality = patient.address.village!!,
                post_town = patient.address.town!!,
                county = patient.address.county!!,
                postcode = patient.address.postcode!!)
        return respondWith(HttpStatus.SC_OK) {
            andJsonBody(GetDemographicsResponseModel(response), GsonFactory.asIs)
        }
    }
}