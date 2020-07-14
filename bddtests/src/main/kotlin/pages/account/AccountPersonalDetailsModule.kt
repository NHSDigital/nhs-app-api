package pages.account

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsVisible
import pages.text

class AccountPersonalDetailsModule(page : HybridPageObject) {

    private val usernamePath = "//p//*[@data-sid='user-name']"
    private val dateOfBirthPath = "//p//*[@data-sid='user-date-of-birth']"
    private val nhsNumberPath = "//p//*[@data-sid='user-nhs-number']"

    private val usernameText = HybridPageElement(
            webDesktopLocator = usernamePath,
            androidLocator = null,
            page = page
    )

    private val dateOfBirthText = HybridPageElement(
            webDesktopLocator = dateOfBirthPath,
            androidLocator = null,
            page = page
    )

    private val nhsNumberText = HybridPageElement(
            webDesktopLocator = nhsNumberPath,
            androidLocator = null,
            page = page
    )

    fun assertVisible(expectedUsername: String,
                      expectedDateOfBirth: String,
                      expectedNhsNumber: String) {
        usernameText.assertIsVisible()
        dateOfBirthText.assertIsVisible()
        nhsNumberText.assertIsVisible()

        val actualUsername = usernameText.text.trim().toLowerCase()
        val actualDateOfBirth = dateOfBirthText.text.trim().toLowerCase()
        val actualNhsNumber = nhsNumberText.text.trim()

        Assert.assertEquals("Username", expectedUsername.toLowerCase(), actualUsername)
        Assert.assertEquals("Date Of Birth", expectedDateOfBirth.toLowerCase(), actualDateOfBirth)
        Assert.assertEquals("NHS Number", expectedNhsNumber, actualNhsNumber)
    }
}
