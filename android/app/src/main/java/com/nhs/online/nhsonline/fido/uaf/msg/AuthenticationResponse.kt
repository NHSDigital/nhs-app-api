package com.nhs.online.nhsonline.fido.uaf.msg

data class AuthenticationResponse(
    var header: OperationHeader? = null,
    var fcParams: String? = null,
    var assertions: List<AuthenticatorSignAssertion?> = emptyList()
)
