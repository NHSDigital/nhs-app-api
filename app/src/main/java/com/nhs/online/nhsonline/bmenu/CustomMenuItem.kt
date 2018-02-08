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


class CustomMenuItem @JvmOverloads constructor(
    context: Context,
    attrs: AttributeSet? = null,
    defStyleAttr: Int = 0
) : LinearLayout(context, attrs, defStyleAttr), View.OnClickListener {
    @DrawableRes
    private var iconId: Int = 0
    @DrawableRes
    private var activeIconId: Int = 0
    private var isActive: Boolean = false
    private lateinit var title: String
    private lateinit var viewTitle: TextView
    private lateinit var viewImage: ImageView

    private var defaultIconWidth: Int = 0
    private var defaultIconHeight: Int = 0
    private var defaultTextSize: Int = 0

    private var menuItemClickedListener: CustomMenu.OnMenuItemClickedListener? = null
    private var itemPosition: Int = 0

    init {
        orientation = LinearLayout.VERTICAL
        isClickable = true
        setOnClickListener(this)
        fetchDataFromXML(context, attrs)
        setDefaultViewValues()
    }

    private fun setDefaultViewValues() {
        defaultIconHeight = convertDPToPixel(18)
        defaultIconWidth = convertDPToPixel(18)
        defaultTextSize = convertSPToPixel(7)
    }

    private fun fetchDataFromXML(context: Context, attrs: AttributeSet?) {
        val array = context.obtainStyledAttributes(attrs, R.styleable.CustomMenuItem)
        iconId = array.getResourceId(R.styleable.CustomMenuItem_inactiveIcon, iconId)
        activeIconId = array.getResourceId(R.styleable.CustomMenuItem_activeIcon, activeIconId)
        title = array.getString(R.styleable.CustomMenuItem_menuTitle)!!.toUpperCase()
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
        val params = LinearLayout.LayoutParams(defaultIconWidth, defaultIconHeight)
        params.gravity = Gravity.CENTER
        params.bottomMargin = convertDPToPixel(2)
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
        viewTitle.setTextSize(TypedValue.COMPLEX_UNIT_PX, defaultTextSize.toFloat())
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
        return if (!isActive) iconId else if (activeIconId == 0) iconId else activeIconId
    }

    private fun convertDPToPixel(dp: Int): Int {
        val sdp = resources.getDimension(R.dimen._1sdp)
        return (dp * sdp).toInt()
    }

    private fun convertSPToPixel(sp: Int): Int {
        val ssp = resources.getDimension(R.dimen._1ssp)
        return (sp * ssp).toInt()
    }

    fun setItemPosition(itemPosition: Int) {
        this.itemPosition = itemPosition
    }

    fun setMenuItemClickedListener(menuItemClickedListener: CustomMenu.OnMenuItemClickedListener) {
        this.menuItemClickedListener = menuItemClickedListener
    }

    override fun onClick(view: View) {
        if (menuItemClickedListener == null) return
        menuItemClickedListener?.onMenuItemClicked(this, itemPosition)
    }
}
