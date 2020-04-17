package com.nhs.online.nhsonline.clients

import java.net.URL

class HttpClient {
    fun readText(url: String): String {
        return URL(url).readText()
    }
}