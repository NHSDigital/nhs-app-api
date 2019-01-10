import Foundation
import os.log

class FidoKeyManager {
    
    @available(iOS 10.0, *)
    func generateKeyPair() throws -> KeyPair {
        deletePrivateKey()
        let attributes = try getAttributes()
        
        var error: Unmanaged<CFError>?
        guard let privateKey = SecKeyCreateRandomKey(attributes as CFDictionary, &error) else {
            throw error!.takeRetainedValue() as Error
        }
        
        if let publicKey = SecKeyCopyPublicKey(privateKey) {
            return  KeyPair(pubKey: publicKey, privKey: privateKey)
        }
        
        throw FidoError.invalidBiometrics
        
    }
    
    func deletePrivateKey() {
        SecItemDelete(getQuery() as CFDictionary)
    }
    
    func getPrivateKey() throws -> SecKey {
        var item: CFTypeRef?
        SecItemCopyMatching(getQuery() as CFDictionary, &item)
        
        if item != nil {
            return item as! SecKey
        }
        throw FidoError.keyRetrievalError
    }
    
    @available(iOS 10.0, *)
    func getSignature(dataToSign: [UInt8], key: SecKey) throws -> [UInt8] {
        let data = Data(bytes: dataToSign, count: dataToSign.count)
        
        guard let signData = SecKeyCreateSignature(key, SecKeyAlgorithm.ecdsaSignatureMessageX962SHA256, data as CFData, nil) else {
            os_log("priv ECC error signing", log: OSLog.default, type: .error)
            throw FidoError.invalidBiometrics
        }
        let signedData = signData as Data
        
        return  [UInt8](signedData)
    }
    
    func getQuery() -> [String: Any]{
        let keyQuery: [String: Any] = [
            kSecClass as String: kSecClassKey,
            kSecAttrKeyClass as String: kSecAttrKeyClassPrivate,
            kSecAttrLabel as String: config().PrivateKeyLabel,
            kSecReturnRef as String: true,
            ]
        return keyQuery
    }
    
    func getAttributes() throws -> [String: Any] {
        return [
            kSecAttrKeyType as String: kSecAttrKeyTypeEC,
            kSecAttrKeySizeInBits as String: 256,
            kSecAttrLabel as String: config().PrivateKeyLabel,
            kSecAttrTokenID as String: kSecAttrTokenIDSecureEnclave,
            kSecPrivateKeyAttrs as String:
                [
                    kSecAttrIsPermanent as String: true,
                    kSecAttrAccessControl as String: try getAccessControlSettings()
                ]
        ]
    }
    
    func getAccessControlSettings() throws -> SecAccessControl {
        var biometricType : SecAccessControlCreateFlags = .touchIDCurrentSet
        
        if #available(iOS 11.3, *) {
            biometricType = .biometryCurrentSet
        }
        
        if let access = SecAccessControlCreateWithFlags(kCFAllocatorDefault,
                                            kSecAttrAccessibleWhenUnlockedThisDeviceOnly,
                                            [.privateKeyUsage, biometricType], nil) {
            return access
        }
        
        throw FidoError.invalidBiometrics
    }
    
}
