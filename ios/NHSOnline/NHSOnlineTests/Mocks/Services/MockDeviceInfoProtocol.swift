import DeviceKit
@testable import NHSOnline

class MockDeviceInfoProtocol: DeviceInfoProtocol {
    var description: Device
    var version: Int
    var identifier: String
    
    init(description: Device, version: Int, identifier: String) {
        self.description = description
        self.version = version
        self.identifier = identifier
    }
    
    func getIOSVersion() -> Int {
        return version
    }
    
    func getDeviceDescription() -> Device {
        return description
    }
    
    func getDeviceIdentifier() -> String {
        return identifier
    }

}
