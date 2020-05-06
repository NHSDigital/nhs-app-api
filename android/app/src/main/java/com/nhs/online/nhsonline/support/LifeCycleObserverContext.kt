package com.nhs.online.nhsonline.support

interface LifeCycleObserverContext {
    val url: String?

    fun showBlankScreen()
    fun hideBlankScreen()
    fun ensureSupportedVersion()
    fun getString(resId: Int): String
}