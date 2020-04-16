package com.nhs.online.nhsonline.clients

import java.net.HttpURLConnection
import java.net.URL

class HttpClient {
    fun readText(url: String, timeoutMilliseconds: Int): String {
        var connection: HttpURLConnection? = null
        try {
            connection = URL(url).openConnection() as HttpURLConnection

            connection.readTimeout = timeoutMilliseconds
            connection.connectTimeout = timeoutMilliseconds
            connection.connect()
            if (isSuccessStatusCode(connection.responseCode)) {
                return connection.inputStream.bufferedReader(Charsets.UTF_8).use { it.readText() }
            }
        } finally {
            connection?.disconnect()
        }
        return ""
    }

    private fun isSuccessStatusCode(responseCode: Int): Boolean {
        if(responseCode in 200..299) {
            return true
        }
        return false
    }
}