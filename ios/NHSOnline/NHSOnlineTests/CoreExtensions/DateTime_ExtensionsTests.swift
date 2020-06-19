import XCTest

class DateTime_ExtensionsTests : XCTestCase {
    override func setUp() {
        super.setUp()
    }
    
    func test_Iso8601_ConvertsToFormatDateString() {
        let formatter = DateFormatter()
        formatter.dateFormat = "yyyy/MM/dd HH:mm"
        let dateTime = formatter.date(from: "2020/01/01 00:01")
        
        let dateString = dateTime!.iso8601
        
        XCTAssertEqual(dateString, "2020-01-01T00:01:00Z")
    }
}
