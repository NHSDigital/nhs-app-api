package com.nhs.online.nhsonline.support

import android.app.Activity
import android.content.Context
import android.content.res.Resources
import android.graphics.Typeface.BOLD
import android.text.SpannableStringBuilder
import android.text.style.StyleSpan
import android.widget.TextView
import com.nhaarman.mockito_kotlin.doReturn
import com.nhaarman.mockito_kotlin.mock
import com.nhs.online.nhsonline.R
import junit.framework.Assert
import org.junit.Before
import org.junit.Test

import org.junit.runner.RunWith
import org.robolectric.Robolectric.buildActivity
import org.robolectric.RobolectricTestRunner

@RunWith(RobolectricTestRunner::class)

class ExtensionsKtTest {
    private lateinit var context: Context

    @Before
    fun setUp() {
        context = mockContext()
    }

    @Test
    fun textviewExtensionSetServiceErrorWithHeader_SetsTextviewTextToHeaderText() {
        val textView = createTextView()
        val header = context.resources.getString(R.string.service_unavailable)
        textView.setServiceError(header)
        Assert.assertEquals(header, textView.text.toString())

    }

    @Test
    fun textviewExtensionSetServiceErrorWithMessage_SetInfoText() {
        val textView = createTextView()
        val header = context.resources.getString(R.string.connection_error_title)
        val info = context.resources.getString(R.string.connection_error_message)
        textView.setServiceError(header, info)
        Assert.assertTrue(textView.text.contains(info))
    }

    private fun createTextView(): TextView {
        val activity = buildActivity(Activity::class.java).create().get()
        return TextView(activity)
    }

    @Test
    fun spannableStringBuilderAppendText_StartsFromNewLine() {
        val builder = serviceUnavailableSpannableStringBuilder()
        Assert.assertTrue(builder.startsWith("\n"))
    }

    @Test
    fun spannableStringBuilderExtensionAppendTextParam_IsBold_SetsSpecifiedTextBold() {
        val builder = serviceUnavailableSpannableStringBuilder(isHeader = true)
        val bold = builder.getSpans(0, builder.length, StyleSpan::class.java)[0]
        Assert.assertTrue(bold.style == BOLD)
    }

    @Test
    fun spannableStringBuilderExtensionAppendTextWithNonNegativeSpecifiedBeforeNewLineValue_IsSameAsAdditionEmptyNewLines() {
        val builder = serviceUnavailableSpannableStringBuilder(5)
        Assert.assertEquals(5, countOccurrences("\n", builder))
    }

    @Test
    fun spannableStringBuilderExtensionAppendTextWithNegativeBeforeNewLineValue_DoesAddAnyNewEmptyLines() {
        val builder = serviceUnavailableSpannableStringBuilder(-5)
        Assert.assertEquals(0, countOccurrences("\n", builder))
    }

    private fun serviceUnavailableSpannableStringBuilder(
        lines: Int = 1,
        isHeader: Boolean = false
    ): SpannableStringBuilder {
        val serviceUnavailable = context.resources.getString(R.string.service_unavailable)
        val builder = SpannableStringBuilder()
        builder.appendText(serviceUnavailable, lines, isHeader)
        return builder
    }

    @Test
    fun intExtensionToStringNewLines_generatesZeroNewEmptyLinesWhenIntValueIsZeroOrLess() {
        for (zeroOrNegativeNum in 0 downTo -10) {
            Assert.assertEquals(0, countOccurrences("\n", zeroOrNegativeNum.toStringNewLines()))
        }
    }

    @Test
    fun intExtensionToStringLines_generatesSameNewEmptyLinesAsIntValue() {
        for (positiveNum in 1..10) {
            Assert.assertEquals(positiveNum, countOccurrences("\n", positiveNum.toStringNewLines()))
        }
    }

    private fun countOccurrences(of: String, text: CharSequence): Int {
        if (of.isEmpty()) return 0

        var count = 0
        var replacedText = text
        while (replacedText.contains(of)) {
            replacedText = replacedText.replaceFirst(Regex(of), "")
            count++
        }
        return count
    }

    private fun mockContext(): Context {
        val mockedResource: Resources = mock {
            on { getString(R.string.service_unavailable) } doReturn "Service unavailable"
            on { getString(R.string.connection_error_title) } doReturn "There's an issue with your internet connection"
            on { getString(R.string.connection_error_message) } doReturn "\nCheck your connection and try again. \n\nIf the problem continues and you need to " +
                    "book an appointment or get a prescription now, contact your GP surgery directly." +
                    " For urgent medical advice, call 111."
        }

        return mock { on { resources } doReturn mockedResource }
    }
}