package com.nhs.online.nhsonline.bmenu

import android.content.Context
import android.os.Parcel
import android.os.Parcelable
import android.util.AttributeSet
import android.view.Gravity
import android.view.View
import android.widget.LinearLayout

/**
 * Created by karma.tsering on 06/02/2018.
 */

class MenuBar @JvmOverloads constructor(
    context: Context,
    attrs: AttributeSet? = null,
    defStyleAttr: Int = 0
) : LinearLayout(context, attrs, defStyleAttr) {
    private val menuItemClickedListener: OnMenuItemClickedListener
    private var onMenuItemSelectListener: OnCustomMenuItemSelectedListener? = null
    private lateinit var items: Array<MenuBarItem?>
    private var selectedPos = -1

    init {
        orientation = LinearLayout.HORIZONTAL
        gravity = Gravity.CENTER
        weightSum = 50f
        this.menuItemClickedListener = object : OnMenuItemClickedListener {
            override fun onMenuItemClicked(menuBarItem: MenuBarItem, position: Int) {
                if (selectedPos != position) {
                    if (selectedPos != -1)
                        items[selectedPos]?.deselectItem()

                    menuBarItem.selectItem()
                    selectedPos = position
                }
                if (onMenuItemSelectListener != null)
                    onMenuItemSelectListener!!.onItemSelected(menuBarItem)
            }
        }
    }

    fun setOnMenuItemSelectListener(onMenuItemSelectListener: OnCustomMenuItemSelectedListener) {
        this.onMenuItemSelectListener = onMenuItemSelectListener
    }

    fun removeOnMenuItemSelectListener(): Boolean {
        if (this.onMenuItemSelectListener == null)
            return false
        this.onMenuItemSelectListener = null
        return true
    }

    override fun onFinishInflate() {
        super.onFinishInflate()
        storeMenuItems()
    }

    private fun storeMenuItems() {
        items = arrayOfNulls(childCount)
        for (i in 0 until childCount) {
            val item = getChildAt(i) as MenuBarItem
            item.setMenuItemClickedListener(this.menuItemClickedListener)
            item.setItemPosition(i)
            (item.layoutParams as LinearLayout.LayoutParams).weight = 10f
            items[i] = item

        }
    }

    override fun onRestoreInstanceState(state: Parcelable) {
        if (state !is SavedState) {
            super.onRestoreInstanceState(state)
            return
        }

        selectedPos = state.selectedPosition
        super.onRestoreInstanceState(state.superState)
    }

    override fun onSaveInstanceState(): Parcelable? {
        val superState = super.onSaveInstanceState()
        val ss = SavedState(superState)
        ss.selectedPosition = selectedPos
        return ss
    }

    interface OnCustomMenuItemSelectedListener {
        fun onItemSelected(barItem: MenuBarItem)
    }

    interface OnMenuItemClickedListener {
        fun onMenuItemClicked(menuBarItem: MenuBarItem, position: Int)
    }

    internal class SavedState : View.BaseSavedState {
        var selectedPosition: Int = 0

        constructor(superState: Parcelable) : super(superState)

        constructor(source: Parcel) : super(source) {
            selectedPosition = source.readInt()
        }

        override fun writeToParcel(dest: Parcel, flags: Int) {
            super.writeToParcel(dest, flags)
            dest.writeInt(selectedPosition)
        }

        companion object {

            val CREATOR: Parcelable.Creator<SavedState> = object : Parcelable.Creator<SavedState> {
                override fun createFromParcel(`in`: Parcel): SavedState {
                    return SavedState(`in`)
                }

                override fun newArray(size: Int): Array<SavedState?> {
                    return arrayOfNulls(size)
                }
            }
        }
    }
}
