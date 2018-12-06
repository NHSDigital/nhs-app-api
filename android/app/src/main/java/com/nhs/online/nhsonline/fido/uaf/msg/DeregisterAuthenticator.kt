package com.nhs.online.nhsonline.fido.uaf.msg

data class DeregisterAuthenticator(
    val aaid: String? = null,
    val keyID: String? = null
)
