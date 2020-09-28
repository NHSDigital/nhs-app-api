package com.nhs.online.nhsonline.support

import android.app.Activity
import android.content.Context
import android.content.res.Resources
import android.text.SpannableString
import android.text.SpannableStringBuilder
import android.view.View
import android.widget.TextView
import com.nhaarman.mockitokotlin2.doReturn
import com.nhaarman.mockitokotlin2.mock
import com.nhaarman.mockitokotlin2.verify
import com.nhs.online.nhsonline.R
import com.nhs.online.nhsonline.text.style.ClickableLink
import org.junit.Assert.assertEquals
import org.junit.Assert.assertTrue
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
        val header = context.resources.getString(R.string.service_unavailable_error_title)
        textView.setServiceError(header)
        assertEquals(header, textView.text.toString())

    }

    @Test
    fun textviewExtensionSetServiceErrorWithMessage_SetInfoText() {
        val textView = createTextView()
        val header = context.resources.getString(R.string.connection_error_title)
        val info = context.resources.getString(R.string.connection_error_message)
        textView.setServiceError(header, info)
        assertTrue(textView.text.contains(info))
    }

    @Test
    fun textviewExtensionMakeLinks_setsClickableLinkSpanOnTextView_atLocationOfEachMatchingPair() {
        val nhs111Url = "111.nhs.uk"
        val call111 = "call 111"
        val text = "If you need urgent help, go to $nhs111Url or $call111."
        val nhs111UrlListener: View.OnClickListener = mock()
        val call111Listener: View.OnClickListener = mock()

        val textView = createTextView()
        textView.text = text

        textView.makeLinks(Pair(nhs111Url, nhs111UrlListener), Pair(call111, call111Listener))

        val nhs111Span = (textView.text as SpannableString).getSpans(31, 41, ClickableLink::class.java)[0]
        val call111Span = (textView.text as SpannableString).getSpans(45, 53, ClickableLink::class.java)[0]

        nhs111Span.onClick(textView)
        call111Span.onClick(textView)

        verify(nhs111UrlListener).onClick(textView)
        verify(call111Listener).onClick(textView)
    }

    @Test
    fun textviewExtensionMakeLinks_whenCalledWithNoLinks_doesNothing() {
        val text = "If you need urgent help, go to 111.nhs.uk or call 111."
        val textView = createTextView()
        textView.text = text

        textView.makeLinks()

        assertEquals(text, textView.text)
    }

    @Test
    fun textviewExtensionMakeLinks_whenTextHasNoMatchingLinks_doesNothing() {
        val text = "If you need urgent help, go to 111.nhs.uk or call 111."
        val textView = createTextView()
        textView.text = text

        textView.makeLinks(Pair("text that doesn't match", mock()))

        assertEquals(text, textView.text)
    }

    private fun createTextView(): TextView {
        val activity = buildActivity(Activity::class.java).create().get()
        return TextView(activity)
    }

    @Test
    fun spannableStringBuilderAppendText_StartsFromNewLine() {
        val builder = serviceUnavailableSpannableStringBuilder()
        assertTrue(builder.startsWith("\n"))
    }

    @Test
    fun spannableStringBuilderExtensionAppendTextWithNonNegativeSpecifiedBeforeNewLineValue_IsSameAsAdditionEmptyNewLines() {
        val builder = serviceUnavailableSpannableStringBuilder(5)
        assertEquals(5, countOccurrences("\n", builder))
    }

    @Test
    fun spannableStringBuilderExtensionAppendTextWithNegativeBeforeNewLineValue_DoesAddAnyNewEmptyLines() {
        val builder = serviceUnavailableSpannableStringBuilder(-5)
        assertEquals(0, countOccurrences("\n", builder))
    }

    private fun serviceUnavailableSpannableStringBuilder(
        lines: Int = 1
    ): SpannableStringBuilder {
        val serviceUnavailable = context.resources.getString(R.string.service_unavailable_error_title)
        val builder = SpannableStringBuilder()
        builder.appendText(serviceUnavailable, lines)
        return builder
    }

    @Test
    fun intExtensionToStringNewLines_generatesZeroNewEmptyLinesWhenIntValueIsZeroOrLess() {
        for (zeroOrNegativeNum in 0 downTo -10) {
            assertEquals(0, countOccurrences("\n", zeroOrNegativeNum.toStringNewLines()))
        }
    }

    @Test
    fun intExtensionToStringLines_generatesSameNewEmptyLinesAsIntValue() {
        for (positiveNum in 1..10) {
            assertEquals(positiveNum, countOccurrences("\n", positiveNum.toStringNewLines()))
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
            on { getString(R.string.service_unavailable_error_title) } doReturn "Service unavailable"
            on { getString(R.string.connection_error_title) } doReturn "There's a problem with your internet connection"
            on { getString(R.string.connection_error_message) } doReturn "\nCheck your connection and try again. If the problem continues and you need to " +
                    "book an appointment or get a prescription now, contact your GP surgery directly." +
                    " For urgent medical advice, call 111."
        }

        return mock { on { resources } doReturn mockedResource }
    }
}
