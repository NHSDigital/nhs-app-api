package models

enum class IdentityProofingLevel(val UserInfoValue: String?) {
    P5("P5"),
    // NHSO-9061: Change UserInfoValue to "P9" when Login starts returning it
    P9(null)
}
