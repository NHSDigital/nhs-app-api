import UIKit

extension UIView {
    func hideView() {
        self.constraints.forEach{(constraint) in
            if constraint.firstAttribute == .height {
                constraint.constant = 0
            }
        }
        self.isHidden = true
    }
    
    func showView() {
        let size = CGSize(width: self.bounds.width, height: .infinity)
        let estimatedSize = self.sizeThatFits(size)
        self.constraints.forEach{(constraint) in
            if constraint.firstAttribute == .height {
                constraint.constant = estimatedSize.height
            }
        }
        self.isHidden = false
    }
}
