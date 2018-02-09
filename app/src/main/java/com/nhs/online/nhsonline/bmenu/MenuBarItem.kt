package com.nhs.online.nhsonline.bmenu

import android.content.Context
import android.os.Build
import android.support.annotation.DrawableRes
import android.support.v4.content.ContextCompat
import android.util.AttributeSet
import android.util.TypedValue
import android.view.Gravity
import android.view.View
import android.view.ViewGroup
import android.widget.ImageView
import android.widget.LinearLayout
import android.widget.TextView

import com.nhs.online.nhsonline.R


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
    private val textSize: Float
        get() = resources.getDimension(R.dimen.menuBarItemFontSize)

    private var menuItemClickedListener: MenuBar.OnMenuItemClickedListener? = null
    private var itemPosition: Int = 0

    init {
        orientation = LinearLayout.VERTICAL
        isClickable = true
        setOnClickListener(this)
        fetchDataFromXML(context, attrs)
    }

    private fun fetchDataFromXML(context: Context, attrs: AttributeSet?) {
        val array = context.obtainStyledAttributes(attrs, R.styleable.MenuBarItem)
        inactiveIconResId = array.getResourceId(
            R.styleable.MenuBarItem_inactiveIcon,
            inactiveIconResId
        )
        activeIconResId = array.getResourceId(
            R.styleable.MenuBarItem_activeIcon,
            activeIconResId
        )
        title = array.getString(R.styleable.MenuBarItem_menuTitle)!!.toUpperCase()
        array.recycle()
    }

    override fun onFinishInflate() {
        super.onFinishInflate()
        initiateChildViews()
    }

    private fun initiateChildViews() {
        initiateMenuIcon()
        initiateMenuTitle()

    }

    private fun initiateMenuIcon() {
        viewImage = ImageView(context)
        val params = LinearLayout.LayoutParams(iconWidth, iconHeight)
        params.gravity = Gravity.CENTER
        params.bottomMargin = resources.getDimension(R.dimen.menuBarItemIconBottomMargin).toInt()
        viewImage.layoutParams = params
        viewImage.setImageResource(getImageResourceId())
        addView(viewImage)
    }

    private fun initiateMenuTitle() {
        viewTitle = TextView(context)
        val params = LinearLayout.LayoutParams(
            ViewGroup.LayoutParams.WRAP_CONTENT,
            ViewGroup.LayoutParams.WRAP_CONTENT
        )
        params.gravity = Gravity.CENTER
        viewTitle.layoutParams = params

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
            viewTitle.setTextAppearance(R.style.menu_default_text)
        } else {
            viewTitle.setTextAppearance(context, R.style.menu_default_text)
        }
        viewTitle.setTextSize(TypedValue.COMPLEX_UNIT_PX, textSize)
        viewTitle.text = title
        addView(viewTitle)

    }

    fun selectItem() {
        if (isActive) return
        isActive = true
        val resId = getImageResourceId()
        this.viewImage.setImageResource(resId)
        this.viewTitle.setTextColor(ContextCompat.getColor(context, R.color.activeMenuTextColor))
    }

    fun deselectItem() {
        if (!isActive) return
        isActive = false
        val resId = getImageResourceId()
        this.viewImage.setImageResource(resId)
        this.viewTitle.setTextColor(ContextCompat.getColor(context, R.color.defaultMenuTextColor))
    }

    private fun getImageResourceId(): Int {
        return if (!isActive) inactiveIconResId else if (activeIconResId == 0) inactiveIconResId else activeIconResId
    }

    fun setItemPosition(itemPosition: Int) {
        this.itemPosition = itemPosition
    }

    fun setMenuItemClickedListener(menuItemClickedListener: MenuBar.OnMenuItemClickedListener) {
        this.menuItemClickedListener = menuItemClickedListener
    }

    override fun onClick(view: View) {
        if (menuItemClickedListener == null) return
        menuItemClickedListener?.onMenuItemClicked(this, itemPosition)
    }
}
