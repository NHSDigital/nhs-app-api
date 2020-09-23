import Foundation
import UIKit
import DeviceKit

class DeviceInfoService: DeviceInfoProtocol {
    func getIOSVersion() -> Int {
        return Int(UIDevice().systemVersion.split(separator: ".")[0])!
    }
    
    func getDeviceDescription() -> Device {
        return Device.current
    }
    
    func getDeviceIdentifier() -> String {
        return Device.identifier
    }
}
