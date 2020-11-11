import Foundation

class UserAgentBuilder {
    let appVersion: String
    let deviceService: DeviceInfoProtocol

    public init(appVersion: String, deviceService: DeviceInfoProtocol) {
        self.appVersion = appVersion
        self.deviceService = deviceService;
    }

    private func buildKvp(key: String, value: Any) -> String {
        return "nhsapp-\(key)/\(value)"
    }

    public func buildUserAgent() -> String {
        let version = buildKvp(key: "ios", value: appVersion)
        let model = buildKvp(key: "model", value: deviceService.getDeviceDescription())
        let os = buildKvp(key: "os", value: deviceService.getFullIOSVersion())
        let architecture = buildKvp(key: "architecture", value: deviceService.getDeviceArchitecture())
    
        return " \(version) nhsapp-manufacturer/apple \(model) \(os) \(architecture)"
    }
}
