package com.nhs.online.nhsonline.navigation

import android.content.Context
import android.os.Parcel
import android.os.Parcelable
import android.util.AttributeSet
import android.view.Gravity
import android.view.View
import android.widget.LinearLayout
import com.nhs.online.nhsonline.support.Optional

class MenuBar @JvmOverloads constructor(
    context: Context,
    attributes: AttributeSet? = null,
    defaultStyleResourceId: Int = 0
) : LinearLayout(context, attributes, defaultStyleResourceId) {
    private var selectedPosition = Optional.empty<Int>()

    var menuItemSelectedListener: ((menuBarItem: MenuBarItem) -> Unit)? = null

    init {
        orientation = LinearLayout.HORIZONTAL
        gravity = Gravity.CENTER
    }

    override fun onFinishInflate() {
        super.onFinishInflate()
        weightSum = childCount.toFloat()
        initialiseMenuItems()
    }

    fun switchActiveMenuItemTo(menuBarItemId: Int) {
        for (i in 0 until childCount) {
            val menuBarItem = getMenuBarItemAt(i)
            if (menuBarItem.id == menuBarItemId) {
                onMenuItemClicked(i, false)
                break
            }
        }
    }

    private fun initialiseMenuItems() {
        for (i in 0 until childCount) {
            val menuBarItem = getMenuBarItemAt(i)
            menuBarItem.menuItemClickedListener = { position ->
                onMenuItemClicked(position)
            }
            menuBarItem.setItemPosition(i)
            (menuBarItem.layoutParams as LinearLayout.LayoutParams).weight = 1f
        }
    }

    private fun getMenuBarItemAt(index: Int): MenuBarItem {
        return getChildAt(index) as MenuBarItem
    }

    private fun onMenuItemClicked(position: Int, shouldInvokeListener: Boolean = true) {
        selectedPosition.ifPresent { selectedPosition ->
            if (selectedPosition != position) {
                getMenuBarItemAt(selectedPosition).deselectItem()
                selectMenuItem(position, shouldInvokeListener)
            }
        }

        selectedPosition.ifEmpty {
            selectMenuItem(position)
        }
    }

    private fun selectMenuItem(position: Int, shouldInvokeListener: Boolean = true) {
        val menuBarItem = getMenuBarItemAt(position)

        menuBarItem.selectItem()
        selectedPosition = Optional.of(position)

        if (shouldInvokeListener)
            menuItemSelectedListener?.invoke(getMenuBarItemAt(position))
    }


    override fun onRestoreInstanceState(state: Parcelable) {
        if (state !is SavedState) {
            super.onRestoreInstanceState(state)
            return
        }

        state.selectedPosition.ifPresent { position -> selectMenuItem(position) }

        super.onRestoreInstanceState(state.superState)
    }

    override fun onSaveInstanceState(): Parcelable? {
        val superState = super.onSaveInstanceState()
        return SavedState(superState, selectedPosition)
    }

    internal class SavedState : View.BaseSavedState {
        companion object {
            private const val UNSELECTED_POSITION = -1
        }

        val selectedPosition: Optional<Int>

        constructor(superState: Parcelable, selectedPosition: Optional<Int>) : super(superState) {
            this.selectedPosition = selectedPosition
        }

        constructor(source: Parcel) : super(source) {
            val position = source.readInt()
            selectedPosition = if (position == UNSELECTED_POSITION) {
                Optional.empty()
            } else {
                Optional.of(position)
            }
        }

        override fun writeToParcel(parcel: Parcel, flags: Int) {
            super.writeToParcel(parcel, flags)
            parcel.writeInt(selectedPosition.orElse(UNSELECTED_POSITION))
        }
    }
}
