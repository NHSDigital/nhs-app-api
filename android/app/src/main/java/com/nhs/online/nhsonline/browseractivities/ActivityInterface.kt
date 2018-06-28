package com.nhs.online.nhsonline.browseractivities

import android.content.Context

interface ActivityInterface {
    fun canStart(context: Context, url: String): Boolean
    fun start(context: Context, url: String)
}