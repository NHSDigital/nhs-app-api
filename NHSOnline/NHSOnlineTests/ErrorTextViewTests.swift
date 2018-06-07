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
    
}
