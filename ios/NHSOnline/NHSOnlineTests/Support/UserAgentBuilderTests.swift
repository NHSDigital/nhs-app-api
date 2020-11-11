import DeviceKit
import Foundation
import XCTest

@testable import NHSOnline

class UserAgentBuilderTests: XCTestCase {
    func test_when_buildUserAgent_is_called_then_correct_details_are_returned() {
        let deviceInfoService = MockDeviceInfoProtocol(description: Device.iPhone6s, version: 13, identifier: "iPhone 6s", architecture: "arm64")
        let userAgentBuilder = UserAgentBuilder(appVersion: "1.2.3", deviceService: deviceInfoService)

        let result = userAgentBuilder.buildUserAgent()

        XCTAssertEqual(
            " nhsapp-ios/1.2.3 nhsapp-manufacturer/apple nhsapp-model/iPhone 6s nhsapp-os/13 nhsapp-architecture/arm64",
            result
        )
    }
}
