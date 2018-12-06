package com.nhs.online.nhsonline.fido.uaf.msg


data class TrustedFacets(
    var version: Version? = null,
    var ids: List<String>? = null
)
