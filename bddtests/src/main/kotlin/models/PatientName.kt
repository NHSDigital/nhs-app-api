package models

data class PatientName(
        val title: String = "",
        val firstName: String = "",
        val surname: String = "",
        val callingName: String = ""){

    fun formattedFullName(identityProofingLevel: IdentityProofingLevel): String {
        val fullName = if (identityProofingLevel == IdentityProofingLevel.P9) {
            "$title $firstName $surname"
        } else {
            "$firstName $surname"
        }

        return fullName.trim()
    }}