import DeviceKit

protocol DeviceInfoProtocol {
    func getIOSVersion() -> Int
    func getDeviceDescription() -> Device
    func getDeviceIdentifier() -> String
    func getDeviceArchitecture() -> String
}
