package com.nhs.online.nhsonline.services

import android.app.Activity
import android.util.Log
import com.nhs.online.nhsonline.clients.HttpClient
import com.nhs.online.nhsonline.data.ErrorMessageHandler
import com.nhs.online.nhsonline.data.ErrorType
import com.nhs.online.nhsonline.interfaces.IInteractor
import com.nhs.online.nhsonline.network.ConnectionStateMonitor
import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionModeAdapter
import com.nhs.online.nhsonline.services.knownservices.enums.MenuTabAdapter
import com.nhs.online.nhsonline.services.knownservices.enums.ViewModeAdapter
import com.squareup.moshi.JsonDataException
import com.squareup.moshi.Moshi
import com.squareup.moshi.kotlin.reflect.KotlinJsonAdapterFactory
import java.io.IOException
import java.net.SocketTimeoutException
import java.util.concurrent.Callable

private val TAG = ConfigurationService::class.java.simpleName

class ConfigurationService(
        private val activity: Activity,
        private val configurationUrl: String,
        private val uiInteractor: IInteractor,
        private val errorMessageHandler: ErrorMessageHandler,
        private val httpClient: HttpClient,
        private val connectionStateMonitor: ConnectionStateMonitor
) : Callable<Configuration?> {
    val timeoutMilliseconds: Int = 5000

    private fun getConfigurationResponse(): Configuration? {
        try {
            val jsonString = httpClient.readText(configurationUrl, timeoutMilliseconds)
            return Moshi.Builder()
                    .add(KotlinJsonAdapterFactory())
                    .add(MenuTabAdapter())
                    .add(JavaScriptInteractionModeAdapter())
                    .add(ViewModeAdapter())
                    .build()
                    .adapter(Configuration::class.java)
                    .fromJson(jsonString)
        } catch (e: JsonDataException) {
            Log.e(TAG, "Configuration error: failed to parse response", e)
        } catch (e: IllegalArgumentException) {
            Log.e(TAG, "Configuration error: failed to parse response", e)
        } catch (e: SocketTimeoutException) {
            Log.e(TAG, "Configuration error: connection timeout", e)
        } catch (e: IOException) {
            Log.e(TAG, "Configuration error: connection error", e)
        } catch (e: Exception) {
            Log.e(TAG, "Unexpected exception", e)
        }
        handleError()
        return null
    }

    private fun handleError() {
        val errorMessage = when (connectionStateMonitor.isConnectedToNetwork) {
            true -> errorMessageHandler.getErrorMessage(ErrorType.ApiCallFailure)
            false -> errorMessageHandler.getErrorMessage(ErrorType.NoConnection)
        }

        activity.runOnUiThread {
            uiInteractor.showUnavailabilityError(errorMessage)
        }
    }

    override fun call(): Configuration? {
        return getConfigurationResponse()
    }
}
