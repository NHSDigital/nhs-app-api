import XCTest
import Foundation
import DeviceKit
@testable import NHSOnline

class DeviceServiceTests: XCTestCase {
    var viewControllerMock: UIViewControllerMock?
    var deviceInfoProtocolMock: MockDeviceInfoProtocol?
    
    override func setUp() {
        super.setUp()
        viewControllerMock = UIViewControllerMock()
    }
    
    func test_versionLessThanRequiredIOSVersionButCompatibleShowsAlert() {
        let deviceService = createDeviceService(version: 10,
                            identifier: "iPhone10,1",
                            description: Device.iPhone8)
        
        let compatibilityCheck = deviceService.performCompatibilityCheck()
        
        XCTAssertTrue(compatibilityCheck.canUpdate)
        XCTAssertFalse(compatibilityCheck.correctVersion)
    }
    
    func test_versionLessThanRequiredVersionIncompatibleDevice() {
        let deviceService = createDeviceService(version: 10,
                            identifier: "iPhone3,1",
                            description: Device.iPhone4)
        
        let compatibilityCheck = deviceService.performCompatibilityCheck()
        
        XCTAssertFalse(compatibilityCheck.canUpdate)
        XCTAssertFalse(compatibilityCheck.correctVersion)
    }
    
    
    func test_versionIsRequiredVersion() {
        let deviceService = createDeviceService(version: 11,
                            identifier: "iPhone10,1",
                            description: Device.iPhone8)
        
        let compatibilityCheck = deviceService.performCompatibilityCheck()
        
        XCTAssertTrue(compatibilityCheck.canUpdate)
        XCTAssertTrue(compatibilityCheck.correctVersion)
    }
    
    func test_versionIsGreaterThanRequiredIOSVersion() {
        let deviceService = createDeviceService(version: 12,
                            identifier: "iPhone10,1",
                            description: Device.iPhone8)
        
        let compatibilityCheck = deviceService.performCompatibilityCheck()
        
        XCTAssertTrue(compatibilityCheck.canUpdate)
        XCTAssertTrue(compatibilityCheck.correctVersion)
    }
    
    override func tearDown() {
        super.tearDown()
    }
    
    
    func createDeviceService(version: Int, identifier: String, description: Device) -> DeviceService {
        deviceInfoProtocolMock = MockDeviceInfoProtocol(description: description,
                                                        version: version,
                                                        identifier: identifier,
                                                        architecture: "AChip")

        return DeviceService.init(viewController: viewControllerMock!, deviceInfoProtocol: deviceInfoProtocolMock!)
    }
}
