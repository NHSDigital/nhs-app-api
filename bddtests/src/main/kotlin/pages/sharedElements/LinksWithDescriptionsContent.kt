package pages.sharedElements

class LinksWithDescriptionsContent(override val linkBlockTitle: String,
                                   override val containerXPath: String = "//div[h2[text()='$linkBlockTitle']]",
                                   override val linkStyling: String)
    : ILinksContent {

    private val sections = "$containerXPath//ul/li//a"
    override val links = arrayListOf<LinkContent>()
    override val hasDescriptionBody: Boolean = true

    fun addLink(title: String, description: String): LinksWithDescriptionsContent {
        links.add(LinkContent(title, description))
        return this
    }

    override fun specificLinkXPath(linkTitle: String) :String {
        return "$sections/div[$linkStyling[contains(text(),'$linkTitle')]]"
    }
}
