package com.nhs.online.nhsonline.data

import android.content.Context

class ErrorMessageHandler(val context: Context) {
    fun getErrorMessage(type: ErrorType): ErrorMessage {
        return ErrorMessage(context, type)
    }
}