package com.nhs.online.nhsonline.fido.uaf.msg

data class AuthenticationRequest(
    val header: OperationHeader? = null,
    val challenge: String? = null,
    val policy: Policy? = null
)
