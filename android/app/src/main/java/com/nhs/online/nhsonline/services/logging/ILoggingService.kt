package com.nhs.online.nhsonline.services.logging

interface ILoggingService {
    fun logError(message: String, cause: Exception? = null)
    fun logInfo(message: String)
}
