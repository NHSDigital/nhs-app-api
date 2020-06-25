package com.nhs.online.nhsonline.services.logging

import android.content.Context
import com.android.volley.RequestQueue
import com.android.volley.toolbox.Volley

class VolleyQueueProvider: IVolleyQueueProvider {
    override fun newRequestQueue(context: Context): RequestQueue {
        return Volley.newRequestQueue(context)
    }
}