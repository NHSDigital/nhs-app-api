package mocking.data.organDonation

import mocking.organDonation.models.Address
import mocking.organDonation.models.CodeableConcept
import mocking.organDonation.models.Coding
import mocking.organDonation.models.Identifier
import mocking.organDonation.models.Name
import mocking.organDonation.models.Registration
import mocking.organDonation.models.Resource
import models.Patient

object OrganDonationRegistration {

    fun getOrganDonationRegistrationData(patient: Patient): List<Registration> {

        val resource = Resource(
                id = "AD02745157",
                resourceType = "Registration",
                identifier = listOf(Identifier(
                        system = "https://fhir.nhs.uk/Id/nhs-number",
                        value = patient.nhsNumbers.first())),
                name = listOf(Name(
                        prefix = listOf(patient.title),
                        given = listOf(patient.firstName),
                        family = patient.surname)
                ),
                gender = patient.sex.toString(),
                birthdate = patient.dateOfBirth,
                ethnicCategory = CodeableConcept(
                        listOf(Coding(
                                system = "White - British",
                                code = 1))),
                religiousAffiliation = CodeableConcept(
                        listOf(Coding(
                                system = "Adventist",
                                code = 1001
                        ))
                ),
                address = listOf(Address(
                        line = listOf(
                                patient.address.houseNameFlatNumber!!,
                                patient.address.numberStreet!!),
                        postalCode = patient.address.postcode!!)),
                telecom = listOf(Identifier(
                        system = "phone",
                        value = patient.contactDetails.telephoneNumber!!)),
                organDonationDecision = patient.organDonationDecision,
                faithDeclaration = patient.faithDeclaration.toString()
        )

        return listOf(Registration(resource))
    }
}
