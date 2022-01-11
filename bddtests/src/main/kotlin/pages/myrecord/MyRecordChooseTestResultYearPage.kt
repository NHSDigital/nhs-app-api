package pages.myrecord

import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.assertSingleElementPresent
import pages.assertElementNotPresent
import java.time.OffsetDateTime
import constants.TppConstants

class MyRecordChooseTestResultYearPage: HybridPageObject() {

    private var pageCount = TppConstants.PageCount

    private val currentYear = OffsetDateTime.now().year
    private val startYear = currentYear - 1
    private val endYear = startYear - (pageCount -1)

    private val nextStartYear = endYear - 1
    private val nextEndYear = nextStartYear - (pageCount - 1)

    private val initialPageTitle = HybridPageElement(
            webDesktopLocator = "//h1",
            page = this
    ).withText(" $startYear to $endYear test results ")

    private val nextPageTitle = HybridPageElement(
            webDesktopLocator = "//h1",
            page = this
    ).withText(" $nextStartYear to $nextEndYear test results ")

    private val nextPaginationLink = HybridPageElement(
            webDesktopLocator = "//*[@id=\"nextPagination\"]",
            page = this
    )
    private val previousPaginationLink = HybridPageElement(
            webDesktopLocator = "//*[@id=\"previousPagination\"]",
            page = this
    )

    fun assertInitialTitleVisible() {
        initialPageTitle.assertIsVisible()
    }

    fun assertNextTitleVisible() {
        nextPageTitle.assertIsVisible()
    }

    fun assertNextPaginationVisible() {
        nextPaginationLink.assertSingleElementPresent()
    }

    fun assertNextPaginationNotVisible() {
        nextPaginationLink.assertElementNotPresent()
    }

    fun clickNextPaginationLink() {
        nextPaginationLink.click()
    }

    fun assertPreviousPaginationVisible() {
        previousPaginationLink.assertSingleElementPresent()
    }

    fun assertPreviousPaginationNotVisible() {
        previousPaginationLink.assertElementNotPresent()
    }

    fun clickPreviousPaginationLink() {
        previousPaginationLink.click()
    }

    fun assertFirstMenuElement(): HybridPageElement {
        val locator = "//h2[contains(text(), $startYear)]"
        return HybridPageElement(
                webDesktopLocator = locator,
                page = this,
                helpfulName = title
        ).assertIsVisible()
    }

    fun assertLastMenuElement(): HybridPageElement {
        val locator = "//h2[contains(text(), $endYear)]"
        return HybridPageElement(
                webDesktopLocator = locator,
                page = this,
                helpfulName = title
        ).assertIsVisible()
    }

    fun assertNextFirstMenuElement(): HybridPageElement {
        val locator = "//h2[contains(text(), $nextStartYear)]"
        return HybridPageElement(
                webDesktopLocator = locator,
                page = this,
                helpfulName = title
        ).assertIsVisible()
    }

    fun assertNextLastMenuElement(): HybridPageElement {
        val locator = "//h2[contains(text(), $nextEndYear)]"
        return HybridPageElement(
                webDesktopLocator = locator,
                page = this,
                helpfulName = title
        ).assertIsVisible()
    }

    fun getHeaderElement(title: Int): HybridPageElement {
        val locator = "//h2[contains(text(), $title)]"
        return HybridPageElement(
                webDesktopLocator = locator,
                page = this
        )
    }
}
