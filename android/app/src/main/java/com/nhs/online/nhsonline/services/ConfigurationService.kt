package com.nhs.online.nhsonline.services

import android.util.Log
import com.nhs.online.nhsonline.clients.HttpClient
import com.nhs.online.nhsonline.data.ErrorMessageHandler
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.ConnectionStateMonitor.Companion.isConnectedToNetwork
import com.nhs.online.nhsonline.services.knownservices.enums.MenuTabAdapter
import com.nhs.online.nhsonline.services.knownservices.enums.ViewModeAdapter
import com.squareup.moshi.JsonDataException
import com.squareup.moshi.Moshi
import java.io.IOException
import java.util.concurrent.Callable

private val TAG = ConfigurationService::class.java.simpleName

class ConfigurationService(
        private val configurationUrl: String,
        private val uiInteractor: IInteractor,
        private val errorMessageHandler: ErrorMessageHandler,
        private val httpClient: HttpClient
) : Callable<Configuration?> {

    private fun getConfigurationResponse(): Configuration? {
        try {
            val jsonString = httpClient.readText(configurationUrl)
            return Moshi.Builder()
                    .add(MenuTabAdapter())
                    .add(ViewModeAdapter())
                    .build()
                    .adapter(Configuration::class.java)
                    .fromJson(jsonString)
        } catch (e: JsonDataException) {
            Log.e(TAG, "Configuration error: failed to parse response", e)
        } catch (e: IllegalArgumentException) {
            Log.e(TAG, "Configuration error: failed to parse response", e)
        } catch (e: IOException) {
            Log.e(TAG, "Configuration error: connection error", e)
        }
        handleError()
        return null
    }

    private fun handleError() {
        val errorMessage = when (isConnectedToNetwork) {
            true -> errorMessageHandler.getErrorMessage(ErrorType.ApiCallFailure)
            false -> errorMessageHandler.getErrorMessage(ErrorType.NoConnection)
        }

        uiInteractor.showUnavailabilityError(errorMessage)
    }

    override fun call(): Configuration? {
        return getConfigurationResponse()
    }
}
