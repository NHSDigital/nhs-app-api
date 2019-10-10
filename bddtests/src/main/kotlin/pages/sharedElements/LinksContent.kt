package pages.sharedElements

class LinksContent(override val linkBlockTitle: String,
                   override val containerXPath: String = "//div[h2[text()='$linkBlockTitle']]")
    : ILinksContent {

    private val sections = "$containerXPath//a/span/h2"
    override val links = arrayListOf<LinkContent>()
    override val hasDescriptionBody: Boolean = false

    fun addLink(title: String): LinksContent {
        links.add(LinkContent(title, null))
        return this
    }

    override fun specificLinkXPath(linkTitle: String): String {
        return "$sections[contains(text(),'$linkTitle')]"
    }
}
