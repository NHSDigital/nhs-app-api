package models

import mocking.emis.demographics.Address

data class PatientContactDetails(
        var telephoneFirst: String = "02837483567",
        var telephoneSecond: String = "07737483567",
        var emailAddress: String = "HalleD@fakeemail.com",
        val address: Address = Address(
                houseNameFlatNumber = "99",
                numberStreet = "Fake Street",
                village = "Fake village",
                town = "Fake town",
                county = "Fake county",
                postcode = "AA00 0AA"
        ))
