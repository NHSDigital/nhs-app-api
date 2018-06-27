package pages

class CitizenIDPage : HybridPageObject(Companion.PageType.WEBVIEW_BROWSER) {

    fun login(username: String, password: String) {
        findByXpath("//input[@id='username']").sendKeys(username)
        findByXpath("//input[@id='password']").sendKeys(password)
        findByXpath("//input[@name='login']").click()
    }
}