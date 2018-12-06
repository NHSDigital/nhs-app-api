package com.nhs.online.nhsonline.fido.uaf.msg

data class AuthenticatorSignAssertion(
    val assertionScheme: String? = null,
    val assertion: String? = null,
    val exts: List<Extension>? = null
)
