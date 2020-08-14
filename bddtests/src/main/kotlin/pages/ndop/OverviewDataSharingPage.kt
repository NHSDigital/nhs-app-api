package pages.ndop

import pages.sharedElements.expectedPage.ExpectedPageStructure

class OverviewDataSharingPage : DataSharingPage() {

    override val pageTitle = overviewPageTitle
    override val nextPageTitle = confidentialDataPageTitle
    override val previousPageTitle: String? = null
    override val expectedPageStructure =
            ExpectedPageStructure()
                    .paragraph(dataSharingSubheader)
                    .contents {
                        h2(contentsTitle)
                                .selectedListItem(overviewPageTitle)
                                .listItem(confidentialDataPageTitle)
                                .listItem(choiceApplyPageTitle)
                                .listItem(makeChoicePageTitle)
                    }
                    .paragraph(
                            "Your health records contain a type of data called confidential " +
                                    "patient information. " +
                                    "This data can be used to help with research and planning. " +
                                    "You can choose to stop your confidential patient information being used " +
                                    "for research and planning.")
                    .paragraph("Your choice will only apply to the health and care system in England. " +
                            "This does not apply to health or care services accessed in Scotland," +
                            " Wales or Northern Ireland.")
                    .inset {
                        paragraph("You're choosing if data from your health records is used across the health and" +
                                " care system in England.")

                                .paragraph("You're not choosing if the NHS App uses your data.")
                    }
                    .h2("What is confidential patient information")
                    .paragraph("Confidential patient information is when 2 types of information " +
                            "from your health records are joined together.")
                    .paragraph("The 2 types of information are:")
                    .listItems("something that can identify you")
                    .listItems("something about your health care or treatment")
                    .paragraph("For example, your name joined with what medicine you take.")
                    .paragraph("Identifiable information on its own is used by health and care services to" +
                            " contact patients and this is not confidential patient information.")
                    .h2("How we use your confidential patient information")
                    .h3("Your individual care")
                    .paragraph("Health and care staff may use your confidential patient information " +
                            "to help with your treatment and care. " +
                            "For example, when you visit your GP, they may look at your records for " +
                            "important information about your health.")
                    .h3("Research and planning")
                    .paragraph("Confidential patient information might also be used to:")
                    .listItems("plan and improve health and care services")
                    .listItems("research and develop cures for serious illnesses")
                    .h2("Your choice")
                    .paragraph("You can stop your confidential patient information being " +
                            "used for research and planning.")
                    .paragraph("If you’re happy with your confidential patient information being used for research" +
                            " and planning you do not need to do anything.")
                    .paragraph("Any choice you make will not impact your individual care.")
                    .h2("More options")
                    .paragraph("Visit the NHS website for more information or to read our privacy notice. " +
                            "You can also find out how to make a choice for someone else. " +
                            "For example, if you’re a parent or guardian of a child under the age of 13.")
                    .pagination { next(nextPageTitle) }
}
