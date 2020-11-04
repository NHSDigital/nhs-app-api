import DeviceKit
@testable import NHSOnline

class MockDeviceInfoProtocol: DeviceInfoProtocol {
    var description: Device
    var version: Int
    var identifier: String
    var architecture: String
    
    init(description: Device, version: Int, identifier: String, architecture: String) {
        self.description = description
        self.version = version
        self.identifier = identifier
        self.architecture = architecture
    }
    
    func getIOSVersion() -> Int {
        return version
    }

    func getFullIOSVersion() -> String {
        return "\(version)"
    }

    func getDeviceDescription() -> Device {
        return description
    }
    
    func getDeviceIdentifier() -> String {
        return identifier
    }
    
    func getDeviceArchitecture() -> String {
        return architecture
    }

}
