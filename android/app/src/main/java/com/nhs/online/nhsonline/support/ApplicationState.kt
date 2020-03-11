package com.nhs.online.nhsonline.support

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