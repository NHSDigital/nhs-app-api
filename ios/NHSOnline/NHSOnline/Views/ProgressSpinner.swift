import Foundation
import UIKit

class ProgressSpinner {
    var backgroundLayer: CAShapeLayer = CAShapeLayer()
    var spinnerLayer: CAShapeLayer = CAShapeLayer()

    func show(uiView: UIView) {
        uiView.isUserInteractionEnabled = false
        
        let progressBackgroundHeight = 60
        let progressBackgroundWidth = 60
        
        let progressBackgroundX = ((uiView.frame.size.width / 2) - CGFloat((progressBackgroundWidth / 2)))
        let progressBackgroundY = ((uiView.frame.size.height / 2) - CGFloat((progressBackgroundHeight / 2)))
        
        // In the centre of the screen, draw a circle with a radius of 30
        let backgroundCirclePath = UIBezierPath(arcCenter: CGPoint(x: progressBackgroundWidth / 2, y: progressBackgroundHeight / 2), radius: CGFloat(30), startAngle: CGFloat(0), endAngle: CGFloat(Double.pi * 2), clockwise: true)
        
        backgroundLayer.path = backgroundCirclePath.cgPath
        backgroundLayer.frame = CGRect(origin: CGPoint(x: progressBackgroundX, y: progressBackgroundY), size: CGSize(width: progressBackgroundWidth, height: progressBackgroundHeight))
        // withAlphaComponent sets the opacity to the desired percentage
        backgroundLayer.fillColor = UIColor.white.withAlphaComponent(0.8).cgColor
        
        
        let center = CGPoint(x: (progressBackgroundWidth / 2), y: (progressBackgroundHeight / 2))
        spinnerLayer.frame = CGRect(origin: CGPoint(x: 0, y: 0), size: CGSize(width: progressBackgroundWidth, height: progressBackgroundHeight))
        let radius: CGFloat = CGFloat(20)
        let startAngle: CGFloat = .pi * 0.5
        let endAngle: CGFloat = 0.0
        spinnerLayer.path = UIBezierPath(arcCenter: center,
                                 radius: radius,
                                 startAngle: startAngle,
                                 endAngle: endAngle,
                                 clockwise: true).cgPath
        spinnerLayer.lineWidth = 5.0
        spinnerLayer.lineCap = kCALineCapSquare
        spinnerLayer.strokeColor = #colorLiteral(red: 0, green: 0.368627451, blue: 0.7215686275, alpha: 1)
        spinnerLayer.fillColor = UIColor.clear.cgColor

        let rotateAnimation = CAKeyframeAnimation(keyPath: "transform.rotation")
        rotateAnimation.values = [
            0.0,
            (2.0 * Float.pi)
        ]
        rotateAnimation.repeatCount = .infinity
        rotateAnimation.duration = 1.5

        spinnerLayer.add(rotateAnimation, forKey: "rotateSpinner")
        backgroundLayer.addSublayer(spinnerLayer)
        uiView.layer.addSublayer(backgroundLayer)
    }

    func hide(uiView: UIView) {
        spinnerLayer.removeAllAnimations()
        spinnerLayer.removeFromSuperlayer()
        backgroundLayer.removeFromSuperlayer()
        uiView.isUserInteractionEnabled = true
    }
    
    func resume(uiView: UIView) {
        if spinnerLayer.superlayer != nil {
            show(uiView: uiView)
        }
    }
}
