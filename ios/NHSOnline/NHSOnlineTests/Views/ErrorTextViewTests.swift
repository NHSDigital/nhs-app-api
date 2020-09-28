import XCTest
@testable import NHSOnline

class ErrorTextViewTests: XCTestCase {
    let errorTextView: ErrorTextView = ErrorTextView()

    func test_WhenSettingHeaderToErrorTextView_HeaderTextWithoutAnyNewLinesAdded() {
        let header = "HEADER"
        errorTextView.setServiceError(title: header)
        XCTAssertEqual(header, errorTextView.attributedText.string)
    }

    func test_WhenSettingHeaderAndInfoToErrorTextView_HeaderAndInfoTextSeperatedByANewLineAdded() {
        let header = "HEADER"
        let message = "MESSAGE"
        errorTextView.setServiceError(title: header, message: message)
        XCTAssertEqual("\(header)\n\(message)", errorTextView.attributedText.string)
    }

    func test_When111WebsiteIsPresentItIsShownAsHyperlink() {
        let header = "HEADER"
        let message = "contains 111.nhs.uk string"
        errorTextView.setServiceError(title: header, message: message)
        XCTAssertEqual("\(header)\n\(message)", errorTextView.attributedText.string)

        let nhs111Attributes = errorTextView.attributedText.attributes(at: 16, effectiveRange: nil)
        XCTAssertEqual(nhs111Attributes.count, 3)

        var hyperLinkFound = false;
        for attr in nhs111Attributes {
            if(attr.key.rawValue == "NSLink") {
                hyperLinkFound = true;
                let value = String(describing: attr.value)
                XCTAssertEqual("https://111.nhs.uk", value);
            }
        }
        XCTAssertTrue(hyperLinkFound)
    }

}
