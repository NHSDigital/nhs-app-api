package com.nhs.online.nhsonline.support

class BiometricsInvalidSignatureException : RuntimeException {

    constructor(cause: Throwable) : super(cause)
    constructor(message: String, cause: Throwable) : super(message, cause)

}