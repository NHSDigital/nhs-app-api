package com.nhs.online.nhsonline.interfaces

import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.services.ConfigurationResponse

interface IVolleyCallback {

    fun onSuccess(configurationResponse: ConfigurationResponse)

    fun onError(errorMessage: ErrorMessage)
}
