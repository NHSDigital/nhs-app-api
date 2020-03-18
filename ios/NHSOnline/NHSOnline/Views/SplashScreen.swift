import Foundation
import UIKit

class SplashScreen {
    var backgroundLayer: CAShapeLayer = CAShapeLayer()
    var logoLayer: CAShapeLayer = CAShapeLayer()
    
    func show(uiView: UIView) {
        let width = uiView.frame.size.width
        let height = uiView.frame.size.height
        
        backgroundLayer.path = UIBezierPath(rect: CGRect(x: 0, y: 0, width: width, height: height)).cgPath
        backgroundLayer.frame = uiView.bounds
        
        backgroundLayer.fillColor = #colorLiteral(red: 0, green: 0.368627451, blue: 0.7215686275, alpha: 1)
        
        let logoImage = UIImage(named: "NHS Online - Logo")
        let baseWidth = logoImage?.size.width
        let baseHeight = logoImage?.size.height
        let scaledWidth = Int(baseWidth! * 2)
        let scaledHeight = Int(baseHeight! * 2)
        
        let imageCentreX = uiView.center.x - CGFloat(scaledWidth / 2) - 1
        
        logoLayer.frame = CGRect(origin: CGPoint(x: imageCentreX, y: CGFloat(165)), size: CGSize(width: scaledWidth, height: scaledHeight))
        logoLayer.contents = logoImage?.cgImage
        
        backgroundLayer.addSublayer(logoLayer)
        uiView.layer.addSublayer(backgroundLayer)
    }
    
    func hide() {
        logoLayer.removeFromSuperlayer()
        backgroundLayer.removeFromSuperlayer()
    }
}
