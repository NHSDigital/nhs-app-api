package com.nhs.online.nhsonline.support

import android.view.View
import com.nhs.online.nhsonline.activities.MainActivity
import kotlinx.android.synthetic.main.activity_main.*


class MainActivityViewSwitcher(
    private val activity: MainActivity,
    private var currentView: ActivityView = ActivityView.WEBVIEW
) {
    fun switchTo(page: ActivityView) {
        setViewVisibility(true, page)
        if (page != currentView) {
            setViewVisibility(false, currentView)
            currentView = page
        }
    }

    private fun setViewVisibility(visible: Boolean, page: ActivityView) {
        val visibility = if (visible) View.VISIBLE else View.GONE
        when (page) {
            ActivityView.WEBVIEW ->
                activity.webview.visibility = visibility

            ActivityView.ERROR -> {
                activity.errorViewLayout.visibility = visibility
                activity.enableMenuBar()
            }
        }
    }
}

enum class ActivityView {
    WEBVIEW, ERROR
}