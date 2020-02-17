package com.nhs.online.nhsonline.data

import android.content.res.Resources

class ErrorMessageHandler(val resources: Resources) {
    fun getErrorMessage(type: ErrorType): ErrorMessage {
        return ErrorMessage(resources, type)
    }
}