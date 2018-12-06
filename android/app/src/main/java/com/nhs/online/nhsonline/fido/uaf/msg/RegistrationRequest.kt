package com.nhs.online.nhsonline.fido.uaf.msg

data class RegistrationRequest(
    val header: OperationHeader? = null,
    val challenge: String? = null,
    val username: String = "",
    val policy: Policy? = null
)
