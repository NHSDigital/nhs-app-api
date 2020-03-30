package pages.ndop

import pages.sharedElements.expectedPage.ExpectedPageStructure

class ConfidentialDataSharingPage: DataSharingPage() {

    override val pageTitle: String = confidentialDataPageTitle
    override val previousPageTitle: String = overviewPageTitle
    override val nextPageTitle: String = choiceApplyPageTitle
    override val expectedPageStructure =
            ExpectedPageStructure()
                    .paragraph(dataSharingSubheader)
                    .contents {
                        h2(contentsTitle)
                                .listItem(overviewPageTitle)
                                .selectedListItem(confidentialDataPageTitle)
                                .listItem(choiceApplyPageTitle)
                                .listItem(makeChoicePageTitle)
                    }
                    .paragraph("The NHS collects confidential patient information from:")
                    .listItems("all NHS organisations, trusts and local authorities",
                            "private organisations, such as private hospitals providing NHS funded care")
                    .paragraph("Research bodies and organisations can request access to this information. " +
                            "This includes:")
                    .listItems("university researchers",
                            "hospital researchers",
                            "medical royal colleges",
                            "pharmaceutical companies researching new treatments")
                    .h2("Who cannot use confidential patient information")
                    .paragraph("Access to confidential patient information will not be given for:")
                    .listItems("marketing purposes",
                            "insurance purposes")
                    .paragraph("(unless you request this)")
                    .h2("How confidential patient information is protected")
                    .paragraph("Your confidential patient information is looked after in accordance " +
                            "with good practice and the law.")
                    .paragraph("Every organisation that provides health and care services " +
                            "will take every step to:")
                    .listItems(
                            "keep data secure",
                            "use data that cannot identify you whenever possible",
                            "use data to benefit health and care",
                            "not use data for marketing or insurance purposes (unless you request this)",
                            "make it clear why and how data is being used")
                    .paragraph("All NHS organisations must provide information on the type of data they " +
                            "collect and how it's used. Data release registers are published by NHS Digital " +
                            "and Public Health England, showing records of the data they have shared with " +
                            "other organisations.")
                    .pagination { previous(previousPageTitle).next(nextPageTitle) }
}