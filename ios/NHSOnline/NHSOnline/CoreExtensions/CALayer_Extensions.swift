import UIKit

extension CALayer {
    
    func addTopBorder(color: UIColor, thickness: CGFloat) {
        let border = CALayer()
        border.frame = CGRect(x: 0, y: 0, width: frame.width, height: thickness)
        border.backgroundColor = color.cgColor;
        addSublayer(border)
    }
}
