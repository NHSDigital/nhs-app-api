package com.nhs.online.nhsonline.services.logging

import android.content.Context
import android.os.Build
import com.android.volley.Request
import com.android.volley.RequestQueue
import com.android.volley.Response
import com.nhs.online.nhsonline.R
import com.squareup.moshi.JsonAdapter
import com.squareup.moshi.Moshi
import org.json.JSONObject
import org.threeten.bp.LocalDateTime
import java.util.logging.Level
import java.util.logging.Logger

private val TAG = LoggingService::class.java.simpleName

class LoggingService(val context: Context, volleyQueueProvider: IVolleyQueueProvider) : ILoggingService {
    private val requestQueue: RequestQueue = volleyQueueProvider.newRequestQueue(context)
    private val loggingRequestAdapter: JsonAdapter<LoggingRequest>
    private val url = "${context.getString(R.string.baseApiURL)}${context.getString(R.string.loggerApiPath)}"
    private val logger = Logger.getLogger(TAG)

    init {
        Moshi.Builder().build().also {
            loggingRequestAdapter = it.adapter(LoggingRequest::class.java)
        }
    }

    override fun logError(message: String) {
        log(message, LogLevel.Error )
    }

    override fun logInfo(message: String) {
        log(message, LogLevel.Information )
    }

    private fun log(message: String, logLevel: LogLevel) {
        var logRequest = LoggingRequest(logLevel.name,"Platform:Android-${Build.VERSION.SDK_INT} ${message}", LocalDateTime.now().toString())

        val serializedRequest = loggingRequestAdapter.toJson(logRequest)

        val noResponseRequest = NoResponseRequest(Request.Method.POST, url, JSONObject(serializedRequest),
                Response.Listener {
                    logger.log(Level.INFO, "LogService Request Succeeded")
                },
                Response.ErrorListener { error ->
                    logger.log(Level.WARNING, "LogService Request failed ${error.message}")
                })

        requestQueue.add(noResponseRequest)
    }
}