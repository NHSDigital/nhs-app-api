package pages.organDonation

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://web.local.bitraft.io:3000/organ-donation")
open class OrganDonationFindOutMoreAboutOrgansAndTissuePage: OrganDonationBasePage() {

    override val titleText: String = "About organs and tissue"

    override fun assertDisplayed() {
        assertPageFullyLoaded()
        assertInformationIsDisplayed()
    }

    private fun assertInformationIsDisplayed() {
        OrganDonationDetailsAssertor.withH3Header("Heart", this)
                .assert("Blood being pumped around your body by your heart carries oxygen and nutrients. " +
                        "Without the heart, your body wouldn’t get oxygen. Your heart can be transplanted whole " +
                        "or in some cases the valves (pulmonary and aortic) can be transplanted."
                )
        OrganDonationDetailsAssertor.withH3Header("Lungs", this)
                .assert("Your lungs supply oxygen to your blood and clear carbon dioxide from your body. " +
                        "Without healthy lungs you couldn’t breathe properly."
                )
        OrganDonationDetailsAssertor.withH3Header("Kidneys", this)
                .assert("Your kidneys filter wastes from your blood and convert them to urine. When your kidneys " +
                        "stop working you can develop kidney failure. Harmful wastes and fluids build up in your " +
                        "body and your blood pressure may rise. You can live healthily with one kidney."
                )
        OrganDonationDetailsAssertor.withH3Header("Liver", this)
                .assert("Your liver produces bile to clean out your body. If your liver isn’t working right, " +
                        "you will begin to feel tired, experience nausea, vomiting, decreased appetite, " +
                        "brown urine, or even jaundice - yellowing in the whites of your eyes. Your liver can " +
                        "be transplanted whole or in some cases the cells (hepatocytes) can be transplanted."
                )
        OrganDonationDetailsAssertor.withH3Header("Corneas", this)
                .assert("The cornea lets light into your eyes, without them you wouldn’t be able to see. " +
                        "The gift of sight is precious. Every day 100 people in the UK start to lose " +
                        "their sight. Almost 2 million people in the UK are living with significant " +
                        "sight loss. Your donation can help someone regain their sight."
                )
        OrganDonationDetailsAssertor.withH3Header("Pancreas", this)
                .assert("Your pancreas is in your abdomen. It produces insulin to control your blood sugar levels. " +
                        "If your pancreas is not working correctly your blood sugar level rises, which can lead to " +
                        "diabetes. Your pancreas can be transplanted whole or in some cases the cells (islet cells) " +
                        "can be transplanted."
                )
        OrganDonationDetailsAssertor.withH3Header("Tissue", this)
                .assert("Tissue is a group of cells that carry out a particular job in your body. Tissue donations " +
                        "such as skin, bone and tendons save hundreds of lives every year. One tissue donor can " +
                        "enhance the lives of more than 50 people."
                )
        OrganDonationDetailsAssertor.withH3Header("Small bowel", this)
                .assert("The small bowel (also small intestine) absorbs nutrients and minerals from food we eat. " +
                        "If your small intestine fails, you wouldn’t be able to digest food. You would need " +
                        "to get nutrition from an alternative method, such as through a drip into your vein."
                )
    }
}
