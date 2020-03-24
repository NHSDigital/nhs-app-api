package pages.sharedElements

interface ILinksContent{
    val links: ArrayList<LinkContent>
    val containerXPath: String
    val linkBlockTitle: String
    val hasDescriptionBody: Boolean
    val linkStyling: String
    fun specificLinkXPath(linkTitle: String) :String
}