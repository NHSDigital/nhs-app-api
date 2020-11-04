import DeviceKit

protocol DeviceInfoProtocol {
    func getIOSVersion() -> Int
    func getFullIOSVersion() -> String
    func getDeviceDescription() -> Device
    func getDeviceIdentifier() -> String
    func getDeviceArchitecture() -> String
}
