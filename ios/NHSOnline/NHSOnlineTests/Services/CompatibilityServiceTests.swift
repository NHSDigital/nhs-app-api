import XCTest
import Foundation
import DeviceKit
@testable import NHSOnline

class CompatibilityServiceTests: XCTestCase {
    var viewControllerMock: HomeViewControllerMocks?
    var deviceServiceMock: DeviceService?
    var deviceInfoProtocolMock: DeviceInfoProtocol?
    var compatibilityService: CompatibilityService?
    
    override func setUp() {
        super.setUp()
        viewControllerMock = HomeViewControllerMocks()
        compatibilityService = CompatibilityService(viewController: viewControllerMock!)
        
    }
    
    func test_versionLessThanRequiredIOSVersionButCompatibleShowsAlert() {
        let deviceService = createDeviceService(version: 10,
                            identifier: "iPhone10,1",
                            description: Device.iPhone8)
        viewControllerMock?.deviceService = deviceService
        
        compatibilityService!.check(isCheckEnabled: true)
        
        XCTAssertTrue(compatibilityService!.hasShownUpdateDialog)
        XCTAssertFalse(compatibilityService!.hasShownIncompatibleScreen)
    }
    
    func test_versionLessThanRequiredVersionIncompatibleDevice() {
        let deviceService = createDeviceService(version: 10,
                            identifier: "iPhone3,1",
                            description: Device.iPhone4)
        viewControllerMock?.deviceService = deviceService
        
        compatibilityService?.check(isCheckEnabled: true)
        
        XCTAssertFalse(compatibilityService!.hasShownUpdateDialog)
        XCTAssertTrue(compatibilityService!.hasShownIncompatibleScreen)
    }
    
    
    func test_versionIsRequiredVersion() {
        let deviceService = createDeviceService(version: 11,
                            identifier: "iPhone10,1",
                            description: Device.iPhone8)
        viewControllerMock?.deviceService = deviceService
        
        compatibilityService?.check(isCheckEnabled: true)
        
        XCTAssertFalse(compatibilityService!.hasShownUpdateDialog)
        XCTAssertFalse(compatibilityService!.hasShownIncompatibleScreen)
    }
    
    func test_versionIsGreaterThanRequiredIOSVersion() {
        let deviceService = createDeviceService(version: 12,
                            identifier: "iPhone10,1",
                            description: Device.iPhone8)
        viewControllerMock?.deviceService = deviceService
        
        compatibilityService?.check(isCheckEnabled: true)
        
        XCTAssertFalse(compatibilityService!.hasShownIncompatibleScreen)
        XCTAssertFalse(compatibilityService!.hasShownUpdateDialog)
    }
    
    override func tearDown() {
        super.tearDown()
    }
    
    
    func createDeviceService(version: Int, identifier: String, description: Device) -> DeviceService {
        deviceInfoProtocolMock = MockDeviceInfoProtocol(description: description,
                                                        version: version,
                                                        identifier: identifier)

        return DeviceService.init(viewController: viewControllerMock!, deviceInfoProtocol: deviceInfoProtocolMock!)
    }
}
