package com.nhs.online.nhsonline.fido.uaf.msg

data class ChannelBinding(
    val serverEndPoint: String? = "",
    val tlsServerCertificate: String? = "",
    val tlsUnique: String? = "",
    val cidPubKey: String? = ""
)
