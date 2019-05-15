package com.nhs.online.nhsonline.navigation

import android.content.Context
import android.content.res.TypedArray
import android.support.annotation.DrawableRes
import android.support.v4.content.ContextCompat
import android.support.v4.widget.TextViewCompat
import android.text.TextUtils
import android.util.AttributeSet
import android.view.Gravity
import android.view.View
import android.view.ViewGroup
import android.view.accessibility.AccessibilityNodeInfo
import android.widget.ImageView
import android.widget.LinearLayout
import android.widget.TextView
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.support.ApplicationState
import com.nhs.online.nhsonline.web.NhsWeb


class MenuBarItem @JvmOverloads constructor(
    context: Context,
    attrs: AttributeSet? = null,
    defStyleAttr: Int = 0
) : LinearLayout(context, attrs, defStyleAttr), View.OnClickListener {
    @DrawableRes
    private var inactiveIconResId: Int = 0
    @DrawableRes
    private var activeIconResId: Int = 0
    private var isActive: Boolean = false
    private lateinit var title: String
    private lateinit var viewTitle: TextView
    private lateinit var viewImage: ImageView

    private val iconWidth: Int
        get() = resources.getDimension(R.dimen.menuBarItemIconWidth).toInt()
    private val iconHeight: Int
        get() = resources.getDimension(R.dimen.menuBarItemIconHeight).toInt()

    var menuItemClickedListener: ((position: Int) -> Unit)? = null
    private var itemPosition: Int = 0

    init {
        orientation = LinearLayout.VERTICAL
        isClickable = true
        setOnClickListener(this)
        readResourcesFromLayout(context, attrs)
    }

    private fun readResourcesFromLayout(context: Context, attrs: AttributeSet?) {
        val array = context.obtainStyledAttributes(attrs, R.styleable.MenuBarItem)

        inactiveIconResId = getRequiredResourceId(array, R.styleable.MenuBarItem_inactiveIcon)
        activeIconResId = getRequiredResourceId(array, R.styleable.MenuBarItem_activeIcon)
        title = array.getString(R.styleable.MenuBarItem_menuTitle)

        array.recycle()
    }

    private fun getRequiredResourceId(styledAttributes: TypedArray, index: Int): Int {
        val defaultValue = 0

        val resourceId = styledAttributes.getResourceId(index, defaultValue)
        if (resourceId == defaultValue) {
            throw Exception("Required Resource Not Found")
        }

        return resourceId
    }

    override fun onFinishInflate() {
        super.onFinishInflate()
        initialiseChildViews()
    }

    private fun initialiseChildViews() {
        initialiseMenuIcon()
        initialiseMenuTitle()
    }

    private fun initialiseMenuIcon() {
        val params = LinearLayout.LayoutParams(iconWidth, iconHeight)
        params.gravity = Gravity.CENTER
        params.bottomMargin = resources.getDimension(R.dimen.menuBarItemIconBottomMargin).toInt()

        viewImage = ImageView(context)
        viewImage.layoutParams = params
        viewImage.setImageResource(inactiveIconResId)

        addView(viewImage)
    }

    private fun initialiseMenuTitle() {
        val params = LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.WRAP_CONTENT,
            ViewGroup.LayoutParams.WRAP_CONTENT
        )
        params.gravity = Gravity.CENTER

        viewTitle = TextView(context)
        viewTitle.layoutParams = params
        TextViewCompat.setTextAppearance(viewTitle, R.style.menu_default_text)
        viewTitle.text = title
        viewTitle.maxLines = 1
        viewTitle.ellipsize = TextUtils.TruncateAt.END

        addView(viewTitle)
    }

    fun selectItem() {
        if (isActive) {
            return
        }

        isActive = true
        this.viewImage.setImageResource(activeIconResId)
        this.viewTitle.setTextColor(ContextCompat.getColor(context, R.color.activeMenuTextColor))
    }

    fun deselectItem() {
        if (!isActive) {
            return
        }

        isActive = false
        this.viewImage.setImageResource(inactiveIconResId)
        this.viewTitle.setTextColor(ContextCompat.getColor(context, R.color.defaultMenuTextColor))
    }

    fun setItemPosition(itemPosition: Int) {
        this.itemPosition = itemPosition
    }

    override fun getAccessibilityClassName(): CharSequence {
        return this::javaClass.name
    }

    override fun onInitializeAccessibilityNodeInfo(info: AccessibilityNodeInfo?) {
        val description = if (isActive) "selected. $title" else title
        contentDescription = "$description. ${itemPosition + 1} of 5"

        val collectionItemInfo = AccessibilityNodeInfo.CollectionItemInfo
            .obtain(itemPosition, 1, 0, 1, false, isActive)
        info?.collectionItemInfo = collectionItemInfo
        super.onInitializeAccessibilityNodeInfo(info)
    }

    override fun onClick(view: View) {
        menuItemClickedListener?.invoke(itemPosition)
    }
}
