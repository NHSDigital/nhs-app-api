package com.nhs.online.nhsonline.support

import com.nhs.online.nhsonline.R
import org.threeten.bp.LocalDateTime

class ApplicationState(private val menuTimeout: Long) {

    private var lastInteraction = LocalDateTime.MIN

    fun block() {
        lastInteraction = LocalDateTime.now()
    }

    fun unBlock() {
        lastInteraction = LocalDateTime.MIN
    }

    fun isReady(): Boolean {
        return LocalDateTime.now().minusSeconds(menuTimeout) > lastInteraction
    }
}