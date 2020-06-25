package com.nhs.online.nhsonline.services.logging

import android.content.Context
import com.android.volley.RequestQueue

interface IVolleyQueueProvider {
    fun newRequestQueue(context: Context) : RequestQueue
}