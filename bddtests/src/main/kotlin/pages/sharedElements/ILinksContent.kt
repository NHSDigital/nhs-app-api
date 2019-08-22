package pages.sharedElements

interface ILinksContent{
    val links: ArrayList<LinkContent>
    val containerXPath: String
    val linkBlockTitle: String
    val hasDescriptionBody: Boolean
    fun specificLinkXPath(linkTitle: String) :String
}