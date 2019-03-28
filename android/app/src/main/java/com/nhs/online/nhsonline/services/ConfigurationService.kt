package com.nhs.online.nhsonline.services

import android.content.Context
import android.util.Log
import com.android.volley.*
import com.android.volley.toolbox.StringRequest
import com.android.volley.toolbox.Volley
import com.nhs.online.nhsonline.Application
import com.nhs.online.nhsonline.BuildConfig
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.data.ErrorMessage
import com.nhs.online.nhsonline.data.ErrorMessageHandler
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IVolleyCallback
import com.nhs.online.nhsonline.network.ConnectionStateMonitor.Companion.isConnectedToNetwork
import org.json.JSONException
import org.json.JSONObject

class ConfigurationResponse {
    var isValidConfiguration: Boolean = false
    var isThrottlingEnabled: Boolean = false
    var fidoServerUrl: String = ""
}

class ConfigurationService(private val context: Context) {
    private val mRequestQueue: RequestQueue = Volley.newRequestQueue(context)
    var isInProgress = false
    private val errorMessageHandler = ErrorMessageHandler(context)
    fun getConfiguration(callback: IVolleyCallback) {
        isInProgress = true

        val configurationUrl = String.format(
            context.resources.getString(R.string.baseApiURL)
                    + context.resources.getString(R.string.configurationApiPath),
            BuildConfig.VERSION_NAME)

        // Request a string response from the provided URL.
        val stringReq = StringRequest(Request.Method.GET, configurationUrl,
            Response.Listener<String> { response ->
                handleGetConfigurationResponse(response, callback)
                isInProgress = false
            },
            Response.ErrorListener { error ->
                Log.d(Application.TAG,
                    "${this::class.java.simpleName}: Configuration error: $error")

                if(!isConnectedToNetwork) {
                    callback.onError(errorMessageHandler.getErrorMessage(ErrorType.NoConnection))
                } else {
                    callback.onError(errorMessageHandler.getErrorMessage(ErrorType.ApiCallFailure))
                }

                isInProgress = false
            })

        mRequestQueue.add(stringReq)
    }

    fun handleGetConfigurationResponse(response: String, callback: IVolleyCallback) {
        try {
            val isValidConfiguration = parseBoolean(response, R.string.isSupportedVersion)
            val isThrottlingEnabled = parseBoolean(response, R.string.isThrottlingEnabled)
            val fidoServerUrl = parseString(response, R.string.fidoServerUrlConfigurationKey)

            val configurationResponse = ConfigurationResponse()
            configurationResponse.isThrottlingEnabled = isThrottlingEnabled
            configurationResponse.isValidConfiguration = isValidConfiguration
            configurationResponse.fidoServerUrl = fidoServerUrl

            Log.d(Application.TAG,
                "${this::class.java.simpleName}: Configuration success: isValidConfiguration " +
                        "${configurationResponse.isValidConfiguration}. " +
                        "Throttling enabled: ${configurationResponse.isThrottlingEnabled}.")

            callback.onSuccess(configurationResponse)
        } catch (error: ClassCastException) {
            Log.d(Application.TAG,
                "${this::class.java.simpleName}: Configuration error: failed to parse response")

            callback.onError(errorMessageHandler.getErrorMessage(ErrorType.ApiCallFailure))
        } catch (error: JSONException) {
            Log.d(Application.TAG,
                "${this::class.java.simpleName}: Configuration error: failed to parse response")

            callback.onError(errorMessageHandler.getErrorMessage(ErrorType.ApiCallFailure))
        }
    }

    private fun parseBoolean(response: String, propertyId: Int): Boolean {
        val jsonObj = JSONObject(response)

        val propertyName = context.resources.getString(propertyId)

        if (jsonObj.has(propertyName)) {
            return jsonObj.getBoolean(propertyName)
        }

        return false
    }

    private fun parseString(response: String, propertyId: Int): String {
        val jsonObj = JSONObject(response)

        val propertyName = context.resources.getString(propertyId)

        if (jsonObj.has(propertyName)) {
            return jsonObj.getString(propertyName)
        }

        return ""
    }
}