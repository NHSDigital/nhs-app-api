package com.nhs.online.nhsonline.interfaces

import com.nhs.online.nhsonline.data.ErrorMessage

interface IVolleyCallback {

    fun onSuccess(isValid: Boolean)

    fun onError(errorMessage: ErrorMessage)
}