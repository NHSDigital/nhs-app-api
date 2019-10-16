package pages.linkedProfiles

import models.linkedProfiles.LinkedProfileOption
import net.thucydides.core.annotations.DefaultUrl
import org.openqa.selenium.By
import org.openqa.selenium.NotFoundException
import org.openqa.selenium.WebElement
import pages.HybridPageObject
import pages.navigation.HeaderNative
import java.util.ArrayList

@DefaultUrl("http://web.local.bitraft.io:3000/linked-profiles")
open class LinkedProfilesPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    private var linkedAccountsXpath = "//li[@data-sid='linked-account']"
    private var nameInsideLinkedProfileXpath: String = ".//span//span"
    private var dateOfBirthInsideLinkedProfileXpath = ".//div[@data-sid='date-of-birth']"

    fun isLoaded() {
        headerNative.waitForPageHeaderText("Linked profiles")
    }

    fun getDisplayedLinkedProfiles(): ArrayList<LinkedProfileOption> {
        val linkedProfiles = arrayListOf<LinkedProfileOption>()
        val linkedProfileLinks = getAllLinkedAccounts()

        linkedProfileLinks.forEach { profileLink ->
            val profile = convertToLinkedProfileOption(profileLink)
            linkedProfiles.add(profile)
        }
        return linkedProfiles
    }

    fun selectLinkedProfile(nameOfProfileToSelect: String) {
        val linkedProfileLinks = getAllLinkedAccounts()

        linkedProfileLinks.forEach { profileLink ->
            val displayedName = profileLink.findElement<WebElement>(
                    By.xpath(nameInsideLinkedProfileXpath)
            )

            if (nameOfProfileToSelect == displayedName.text) {
                profileLink.click()
                return
            }
        }

        throw NotFoundException("Couldn't find profile to select with name $nameOfProfileToSelect")
    }

    private fun convertToLinkedProfileOption(element: WebElement): LinkedProfileOption {
        val name = element.findElement<WebElement>(
                By.xpath(nameInsideLinkedProfileXpath)
        )

        val dateOfBirth = element.findElement<WebElement>(
                By.xpath(dateOfBirthInsideLinkedProfileXpath))

        return LinkedProfileOption(name = name.text, dateOfBirth = dateOfBirth.text)
    }

    private fun getAllLinkedAccounts(): List<WebElement> {
        return findAllByXpath(linkedAccountsXpath)
    }
}
