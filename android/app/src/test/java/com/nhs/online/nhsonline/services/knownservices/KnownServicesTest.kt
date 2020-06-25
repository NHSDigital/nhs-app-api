package com.nhs.online.nhsonline.services.knownservices

import com.nhs.online.nhsonline.services.knownservices.enums.JavaScriptInteractionMode
import com.nhs.online.nhsonline.services.knownservices.enums.MenuTab
import com.nhs.online.nhsonline.services.knownservices.enums.ViewMode
import org.junit.Assert
import org.junit.Test
import org.junit.runner.RunWith
import org.robolectric.RobolectricTestRunner
import java.net.URL

@RunWith(RobolectricTestRunner::class)
class KnownServicesTest {
    private val testKnownServices = mockKnownServices()

    @Test
    fun findMatchingSubService_ResolveToNull_WhenUrlIsInvalid() {
        val result = testKnownServices.findMatchingKnownService(URL("https://invalidurl.com"))

        Assert.assertNull(result)
    }

    @Test
    fun findMatchingSubService_ResolveToRootService_WhenUrlHasNoPathAndQuery() {
        val url = "https://test.com"
        val result = testKnownServices.findMatchingKnownService(URL(url))

        Assert.assertNotNull(result)
        Assert.assertTrue(result is RootService)
        Assert.assertTrue((result as RootService).url == url)
    }

    @Test
    fun findMatchingSubService_ResolveToRootService_WhenUrlHasInvalidPath() {
        val url = "https://test.com/pathnotvalid/"
        val result = testKnownServices.findMatchingKnownService(URL(url))

        Assert.assertNotNull(result)
        Assert.assertTrue(result is RootService)
    }

    @Test
    fun findMatchingSubService_ResolveToRootService_WhenUrlHasInvalidQueryString() {
        val url = "https://test.com/?query=invalid"
        val result = testKnownServices.findMatchingKnownService(URL(url))

        Assert.assertNotNull(result)
        Assert.assertTrue(result is RootService)
    }

    @Test
    fun findMatchingSubService_ResolveToSubService_WhenUrlHasValidPath() {
        val url = "https://test.com/path"
        val result = testKnownServices.findMatchingKnownService(URL(url))

        Assert.assertNotNull(result)
        Assert.assertTrue(result is SubService)
        Assert.assertTrue((result as SubService).path == "/path")
        Assert.assertTrue(result.queryString == null)
    }

    @Test
    fun findMatchingSubService_ResolveToSubService_WhenUrlHasPartialValidPath() {
        val url = "https://test.com/path/subpath"
        val result = testKnownServices.findMatchingKnownService(URL(url))

        Assert.assertNotNull(result)
        Assert.assertTrue(result is SubService)
        Assert.assertTrue((result as SubService).path == "/path")
        Assert.assertTrue(result.queryString == null)
    }

    @Test
    fun findMatchingSubService_ResolveToSubService_WhenUrlHasValidQueryString() {
        val url = "https://test.com/?foo=bar"
        val result = testKnownServices.findMatchingKnownService(URL(url))

        Assert.assertNotNull(result)
        Assert.assertTrue(result is SubService)
        Assert.assertTrue((result as SubService).path == null)
        Assert.assertTrue(result.queryString == "foo=bar")
    }

    @Test
    fun findMatchingSubService_ResolveToSubService_WhenUrlHasValidQueryStringUnmatchedPath() {
        val url = "https://test.com/dave?foo=bar"
        val result = testKnownServices.findMatchingKnownService(URL(url))

        Assert.assertNotNull(result)
        Assert.assertTrue(result is SubService)
        Assert.assertTrue((result as SubService).path == null)
        Assert.assertTrue(result.queryString == "foo=bar")
    }

    @Test
    fun findMatchingSubService_ResolveToSubService_WhenUrlHasValidPathAndQueryString() {
        val url = "https://test.com/path?foo=bar"
        val result = testKnownServices.findMatchingKnownService(URL(url))

        Assert.assertNotNull(result)
        Assert.assertTrue(result is SubService)
        Assert.assertTrue((result as SubService).path == "/path")
        Assert.assertTrue(result.queryString == "foo=bar")
    }

    @Test
    fun findMatchingSubService_ResolveToSubService_WhenUrlHasPartialValidPathAndQueryString() {
        val url = "https://test.com/path/nestedpath?foo=bar&ram=you"
        val result = testKnownServices.findMatchingKnownService(URL(url))

        Assert.assertNotNull(result)
        Assert.assertTrue(result is SubService)
        Assert.assertTrue((result as SubService).path == "/path")
        Assert.assertTrue(result.queryString == "foo=bar")
    }

    @Test
    fun findMatchingSubService_ResolveToSubService_WhenUrlHasPartialValidPathAndInvalidQueryString() {
        val url = "https://test.com/path/nestedpath?aaa=bbb"
        val result = testKnownServices.findMatchingKnownService(URL(url))

        Assert.assertNotNull(result)
        Assert.assertTrue(result is SubService)
        Assert.assertTrue((result as SubService).path == "/path")
        Assert.assertTrue(result.queryString == null)
    }

    @Test
    fun findMatchingSubService_ResolveToSubService_WhenUrlHasValidMultipleQueryStringParamaters() {
        val url = "https://test.com/?foo=bar&bar=ram"
        val result = testKnownServices.findMatchingKnownService(URL(url))

        Assert.assertNotNull(result)
        Assert.assertTrue(result is SubService)
        Assert.assertTrue((result as SubService).path == null)
        Assert.assertTrue(result.queryString == "foo=bar&bar=ram")
    }

    @Test
    fun findMatchingSubService_ResolveToSubService_WhenUrlHasValidMultipleQueryStringParamatersInDifferentOrder() {
        val url = "https://test.com/?bar=ram&foo=bar"
        val result = testKnownServices.findMatchingKnownService(URL(url))

        Assert.assertNotNull(result)
        Assert.assertTrue(result is SubService)
        Assert.assertTrue((result as SubService).path == null)
        Assert.assertTrue(result.queryString == "foo=bar&bar=ram")
    }

    @Test
    fun findMatchingSubService_ResolveToSubService_WhenUrlHasValidTwoLevelPath() {
        val url = "https://test.com/path/valid-subpath"
        val result = testKnownServices.findMatchingKnownService(URL(url))

        Assert.assertNotNull(result)
        Assert.assertTrue(result is SubService)
        Assert.assertTrue((result as SubService).path == "/path/valid-subpath")
        Assert.assertTrue(result.queryString == null)
    }

    private fun mockKnownServices(): KnownServices {
        val testSubServices = listOf(
                SubService(requiresAssertedLoginIdentity = false, validateSession = false, menuTab = MenuTab.Unknown, viewMode = ViewMode.Unknown, showSpinner = false, javaScriptInteractionMode = JavaScriptInteractionMode.Unknown, path = null, queryString = "foo=bar"),
                SubService(requiresAssertedLoginIdentity = false, validateSession = true, menuTab = MenuTab.Unknown, viewMode = ViewMode.Unknown, showSpinner = false, javaScriptInteractionMode = JavaScriptInteractionMode.Unknown, path = "/path", queryString = null),
                SubService(requiresAssertedLoginIdentity = false, validateSession = true, menuTab = MenuTab.Unknown, viewMode = ViewMode.Unknown, showSpinner = false, javaScriptInteractionMode = JavaScriptInteractionMode.Unknown, path = "/path/valid-subpath", queryString = null),
                SubService(requiresAssertedLoginIdentity = true, validateSession = false, menuTab = MenuTab.Unknown, viewMode = ViewMode.Unknown, showSpinner = false, javaScriptInteractionMode = JavaScriptInteractionMode.Unknown, path = "/path", queryString = "foo=bar"),
                SubService(requiresAssertedLoginIdentity = true, validateSession = true, menuTab = MenuTab.Unknown, viewMode = ViewMode.Unknown, showSpinner = false, javaScriptInteractionMode = JavaScriptInteractionMode.Unknown, path = null, queryString = "foo=bar&bar=ram")
        )

        val rootServices = listOf(
                RootService(requiresAssertedLoginIdentity = true, validateSession = true, menuTab = MenuTab.Unknown, viewMode = ViewMode.Unknown, showSpinner = false, javaScriptInteractionMode = JavaScriptInteractionMode.Unknown, url = "https://test.com", subServices = testSubServices)
        )

        return KnownServices(rootServices)
    }
}