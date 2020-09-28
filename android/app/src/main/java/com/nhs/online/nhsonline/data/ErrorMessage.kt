package com.nhs.online.nhsonline.data

import android.content.res.Resources
import com.nhs.online.nhsonline.R

class ErrorMessage(resources: Resources, errorType: ErrorType) {
    var header: String = ""
    var title: String
    var message:String?
    var accessibleMessage:String?
    var isRetry:Boolean

    init {
        when(errorType){
            ErrorType.NoConnection -> {
                header = resources.getString(R.string.connection_error_header)
                title = resources.getString(R.string.connection_error_title)
                message = resources.getString(R.string.connection_error_message)
                accessibleMessage = resources.getString(R.string.accessible_connection_error_message)
                isRetry = true
            }
            ErrorType.ServiceUnavailable -> {
                title = resources.getString(R.string.server_error_title)
                message = resources.getString(R.string.server_error_message)
                accessibleMessage = resources.getString(R.string.accessible_server_error_message)
                isRetry = true
            }
            ErrorType.ApiCallFailure -> {
                title = resources.getString(R.string.service_unavailable_error_title)
                message = resources.getString(R.string.service_unavailable_error_message)
                accessibleMessage = resources.getString(R.string.accessible_service_unavailable_error_message)
                isRetry = true
            }
            ErrorType.BrowserNotAvailable -> {
                title = resources.getString(R.string.browser_unavailable_title)
                message = resources.getString(R.string.browser_unavailable_message)
                accessibleMessage = resources.getString(R.string.accessible_browser_unavailable_message)
                isRetry = true
            }
            ErrorType.DownloadDocumentError -> {
                val downloadFailureMessage = resources.getString(R.string.download_failure_message)
                header = resources.getString(R.string.download_failure_header)
                title = ""
                message = downloadFailureMessage
                accessibleMessage = downloadFailureMessage
                isRetry = false
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
    NoConnection, ServiceUnavailable, ApiCallFailure,
    BrowserNotAvailable, DownloadDocumentError
}
