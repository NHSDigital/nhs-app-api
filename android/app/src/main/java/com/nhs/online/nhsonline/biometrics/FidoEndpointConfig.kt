package com.nhs.online.nhsonline.biometrics

class FidoEndpointConfig(
    private val server: String,
    private val authRequestGet: String,
    private val deregResponsePost: String,
    private val regReguestGet: String,
    private val regResponsePost: String
) {

    fun getAuthRequestGet(): String {
        return server + authRequestGet
    }

    fun getDeregPost(): String {
        return server + deregResponsePost
    }

    fun getRegRequestGet(): String {
        return server + regReguestGet
    }

    fun getRegResponsePost(): String {
        return server + regResponsePost
    }
}
