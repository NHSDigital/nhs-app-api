package pages.ndop

import pages.HybridPageElement
import pages.sharedElements.expectedPage.ExpectedPageStructure

class MakeChoiceDataSharingPage: DataSharingPage() {

    override val pageTitle: String = makeChoicePageTitle
    override val previousPageTitle = choiceApplyPageTitle
    override val nextPageTitle = null
    override val expectedPageStructure =
            ExpectedPageStructure()
                    .paragraph(dataSharingSubheader)
                    .contents {
                        h2(contentsTitle)
                                .listItem(overviewPageTitle)
                                .listItem(confidentialDataPageTitle)
                                .listItem(choiceApplyPageTitle)
                                .selectedListItem(makeChoicePageTitle)
                    }
                    .paragraph("Use this service to:")
                    .listItems("choose if your confidential patient information is used for research and planning",
                            "change or check your current choice")
                    .paragraph("If you want to make a choice for someone else, find out how to on the NHS website.")
                    .paragraph("Your choice will be applied by:")
                    .listItems("NHS Digital and Public Health England",
                            "all other health and care organisations by March 2020")
                    .paragraph("Any choice you make will not impact your individual care.")
                    .inset {
                        paragraph("You're choosing if data from your health records is used across the " +
                            "health and care system in England.")
                            .paragraph("You're not choosing if the NHS App uses your data.")
                    }
                    .button("Start now")
                    .pagination { previous(previousPageTitle) }

    val btnStartNow = HybridPageElement(
            webDesktopLocator = "//button[normalize-space(text())='Start now']",
            androidLocator = null,
            page = this
    )
}