package models.nominatedPharmacy

data class PharmacySearchResult(
        var pharmacyName: String = "",
        var address: String = "",
        var phoneNumber: String = "",
        var distance: String? = null
)