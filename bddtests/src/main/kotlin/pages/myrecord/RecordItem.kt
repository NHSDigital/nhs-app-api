package pages.myrecord

import net.serenitybdd.core.pages.WebElementFacade
import org.openqa.selenium.By

class RecordItem(recordItem: WebElementFacade) {

    val element = recordItem

    val label = recordItem.find<WebElementFacade>(By.cssSelector("[data-purpose='record-item-header']")).text

    val bodyElements = recordItem.thenFindAll(By.cssSelector("[data-purpose='record-item-detail']")).map { element ->
        element.text
    }
}
