package mocking.emis.models

data class Location(
        var locationId: Int,
        var locationName: String,
        var houseNameFlatNumber: String? = null,
        var numberAndStreet: String? = null,
        var village: String? = null,
        var town: String? = null,
        var postcode: String? = null
)
