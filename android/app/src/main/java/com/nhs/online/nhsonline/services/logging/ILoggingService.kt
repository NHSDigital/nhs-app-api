package com.nhs.online.nhsonline.services.logging

interface ILoggingService {
    fun logError(message: String)
    fun logInfo(message: String)
}