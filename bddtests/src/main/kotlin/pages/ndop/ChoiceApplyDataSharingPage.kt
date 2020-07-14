package pages.ndop

import pages.sharedElements.expectedPage.ExpectedPageStructure

class ChoiceApplyDataSharingPage: DataSharingPage() {

    override val pageTitle: String = choiceApplyPageTitle
    override val previousPageTitle: String = confidentialDataPageTitle
    override val nextPageTitle: String = makeChoicePageTitle
    override val expectedPageStructure =
            ExpectedPageStructure()
                    .paragraph(dataSharingSubheader)
                    .contents {
                        h2(contentsTitle)
                                .listItem(overviewPageTitle)
                                .listItem(confidentialDataPageTitle)
                                .selectedListItem(choiceApplyPageTitle)
                                .listItem(makeChoicePageTitle)
                    }
                    .paragraph("If you choose to stop your confidential patient " +
                            "information being used for research and planning, " +
                            "your data might still be used in some situations.")
                    .h2("When required by law")
                    .paragraph("If there’s a legal requirement to provide it, such as a court order.")
                    .h2("When you have given consent")
                    .paragraph("If you have given your consent, such as for a medical research study.")
                    .h2("When there is an overriding public interest")
                    .paragraph("In an emergency or in a situation when the safety of others is most important. " +
                            "For example, to help manage contagious diseases like meningitis and stop them spreading.")
                    .h2("When information that can identify you is removed")
                    .paragraph("Information about your health care or treatment might still be used in research" +
                            " and planning if the information that can identify you is removed first.")
                    .h2("When there is a specific exclusion")
                    .paragraph("Your confidential patient information can still be used in a small number of" +
                            " situations. For example, for official national statistics like a population census.")
                    .pagination { previous(previousPageTitle).next(nextPageTitle) }
}
