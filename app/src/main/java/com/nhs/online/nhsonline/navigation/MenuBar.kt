package com.nhs.online.nhsonline.navigation

import android.content.Context
import android.os.Parcel
import android.os.Parcelable
import android.util.AttributeSet
import android.view.Gravity
import android.view.View
import android.widget.LinearLayout

class MenuBar @JvmOverloads constructor(
        context: Context,
        attributes: AttributeSet? = null,
        defaultStyleResourceId: Int = 0
) : LinearLayout(context, attributes, defaultStyleResourceId) {
    private var selectedPos = 0

    init {
        orientation = LinearLayout.HORIZONTAL
        gravity = Gravity.CENTER
    }

    override fun onFinishInflate() {
        super.onFinishInflate()
        weightSum = childCount.toFloat()

        initialiseMenuItems()
    }

    private fun initialiseMenuItems() {
        for (i in 0 until childCount) {
            val menuBarItem = getMenuBarItemAt(i)
            menuBarItem.menuItemClickedListener = {menuBarItem, position -> onMenuItemClicked(menuBarItem, position) }
            menuBarItem.setItemPosition(i)
            (menuBarItem.layoutParams as LinearLayout.LayoutParams).weight = 1f
        }
    }

    private fun getMenuBarItemAt(index: Int): MenuBarItem {
        return getChildAt(index) as MenuBarItem
    }

    private fun onMenuItemClicked(menuBarItem: MenuBarItem, position: Int) {
        val previousSelectedPosition = selectedPos
        if (previousSelectedPosition != position) {
            if (previousSelectedPosition != null)
                getMenuBarItemAt(previousSelectedPosition).deselectItem()

            menuBarItem.selectItem()
            selectedPos = position
        }
    }

    override fun onRestoreInstanceState(state: Parcelable) {
        if (state !is SavedState) {
            super.onRestoreInstanceState(state)
            return
        }

        selectedPos = state.selectedPosition
        getMenuBarItemAt(selectedPos).selectItem()

        super.onRestoreInstanceState(state.superState)
    }

    override fun onSaveInstanceState(): Parcelable? {
        val superState = super.onSaveInstanceState()
        return SavedState(superState, selectedPos)
    }

    internal class SavedState : View.BaseSavedState {
        val selectedPosition: Int

        constructor(superState: Parcelable, selectedPosition: Int) : super(superState) {
            this.selectedPosition = selectedPosition
        }

        constructor(source: Parcel) : super(source) {
            selectedPosition = source.readInt()
        }

        override fun writeToParcel(parcel: Parcel, flags: Int) {
            super.writeToParcel(parcel, flags)
            parcel.writeInt(selectedPosition)
        }
    }
}
