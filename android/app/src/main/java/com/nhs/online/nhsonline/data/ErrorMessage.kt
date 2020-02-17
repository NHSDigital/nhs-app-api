package com.nhs.online.nhsonline.data

import android.content.Context
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
                accessibleMessage = resources.getString(R.string.Accessible_connection_error_message)
                isRetry = true
            }
            ErrorType.ServiceUnavailable -> {
                title = resources.getString(R.string.server_error_title)
                message = resources.getString(R.string.server_error_message)
                accessibleMessage = resources.getString(R.string.accessible_server_error_message)
                isRetry = true
            }
            ErrorType.ApiCallFailure -> {
                title = resources.getString(R.string.service_unavailable)
                message = resources.getString(R.string.apiUnavailableErrorMessage)
                accessibleMessage = resources.getString(R.string.accessible_apiUnavailableErrorMessage)
                isRetry = true
            }
            ErrorType.BrowserNotAvailable -> {
                title = resources.getString(R.string.browser_unavailable)
                message = resources.getString(R.string.browser_unavailable_message)
                accessibleMessage = resources.getString(R.string.accessible_browser_unavailable_message)
                isRetry = true
            }
            ErrorType.BiometricDeviceFailure -> {
                title = resources.getString(R.string.biometric_header)
                message = resources.getString(R.string.biometric_device_failure_message)
                accessibleMessage = resources.getString(R.string.biometric_device_failure_message)
                isRetry = false
            }
            ErrorType.BiometricRegistrationFailure -> {
                title = resources.getString(R.string.biometric_registration_header)
                message = resources.getString(R.string.biometric_registration_failure_message)
                accessibleMessage = resources.getString(R.string.biometric_device_failure_message)
                isRetry = false
            }
            ErrorType.DownloadDocumentError -> {
                header = resources.getString(R.string.download_failure_title)
                title = ""
                message = resources.getString(R.string.download_failure_message)
                accessibleMessage = resources.getString(R.string.accessible_download_failure_message)
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
    BrowserNotAvailable, BiometricDeviceFailure, BiometricRegistrationFailure,
    DownloadDocumentError
}