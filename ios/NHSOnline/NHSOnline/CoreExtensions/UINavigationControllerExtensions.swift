import UIKit

extension UINavigationController {
    open override var preferredStatusBarStyle: UIStatusBarStyle {
        return .lightContent
    }
    override open var childViewControllerForStatusBarStyle: UIViewController? {
      return topViewController
    }

    override open var childViewControllerForStatusBarHidden: UIViewController? {
      return topViewController
    }
}
