import Foundation
import UIKit
import DeviceKit

class DeviceInfoService: DeviceInfoProtocol {
    func getIOSVersion() -> Int {
        return Int(UIDevice().systemVersion.split(separator: ".")[0])!
    }

    func getFullIOSVersion() -> String {
        return UIDevice().systemVersion
    }
    
    func getDeviceDescription() -> Device {
        return Device.current
    }
    
    func getDeviceIdentifier() -> String {
        return Device.identifier
    }
    
    func getDeviceArchitecture() -> String {
        let archInfo = NXGetLocalArchInfo()
        return String(cString: (archInfo?.pointee.description)!)
    }
}
