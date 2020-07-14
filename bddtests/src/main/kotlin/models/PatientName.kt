package models

data class PatientName(
        val title: String = "",
        val firstName: String = "",
        val surname: String = "",
        val callingName: String = ""){

    fun formattedFullName(identityProofingLevel: IdentityProofingLevel, includeTitle: Boolean): String {
        val fullName = if (includeTitle && identityProofingLevel == IdentityProofingLevel.P9) {
            "$title $firstName $surname"
        } else {
            "$firstName $surname"
        }

        return fullName.trim()
    }
}
