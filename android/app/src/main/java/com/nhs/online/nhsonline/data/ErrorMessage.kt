package com.nhs.online.nhsonline.data

import android.content.Context
import com.nhs.online.nhsonline.R

class ErrorMessage(context: Context, errorType: ErrorType) {
    var title:String
    var message:String?
    var accessibleMessage:String?

    init {
        when(errorType){
            ErrorType.NoConnection -> {
                title = context.resources.getString(R.string.connection_error_title)
                message = context.resources.getString(R.string.connection_error_message)
                accessibleMessage = context.resources.getString(R.string.Accessible_connection_error_message)
            }
            ErrorType.ServiceUnavailable -> {
                title = context.resources.getString(R.string.server_error_title)
                message = context.resources.getString(R.string.server_error_message)
                accessibleMessage = context.resources.getString(R.string.accessible_server_error_message)
            }
            ErrorType.ApiCallFailure -> {
                title = context.resources.getString(R.string.service_unavailable)
                message = context.resources.getString(R.string.apiUnavailableErrorMessage)
                accessibleMessage = context.resources.getString(R.string.accessible_apiUnavailableErrorMessage)
            }
        }
    }

    override fun equals(other: Any?): Boolean {
        val errorMsg = other as ErrorMessage
        return this.title == other.title &&
                errorMsg.message == errorMsg.message &&
                errorMsg.accessibleMessage == other.accessibleMessage
    }

    override fun hashCode(): Int {
        var result = title.hashCode()
        result = 31 * result + (message?.hashCode() ?: 0)
        result = 31 * result + (accessibleMessage?.hashCode() ?: 0)
        return result
    }
}

enum class ErrorType {
    NoConnection, ServiceUnavailable, ApiCallFailure
}