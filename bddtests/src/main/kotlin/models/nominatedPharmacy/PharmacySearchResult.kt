package models.nominatedPharmacy

data class PharmacySearchResult(
        var pharmacyName: String = "",
        var address: String = "",
        var phoneNumber: String = "",
        var distance: String? = null
)

data class OnlinePharmacySearchResult(
        var pharmacyName: String = "",
        var phoneNumber: String? = "",
        var website: String? = ""
)