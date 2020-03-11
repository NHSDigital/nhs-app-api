package com.nhs.online.nhsonline.browseractivities

import android.content.Context
import com.nhs.online.nhsonline.interfaces.IInteractor

interface ActivityInterface {
    fun start(context: Context, url: String, interactor: IInteractor)
}