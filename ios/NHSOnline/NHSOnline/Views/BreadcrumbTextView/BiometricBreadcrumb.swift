import UIKit

class BiometricBreadcrumb: UIView {
    
    @IBOutlet var contentView: UIView!
    @IBOutlet weak var biometricLabel: UILabel!
    @IBOutlet weak var breadcrumbButton: UIButton!
    
    override init(frame: CGRect) {
        super.init(frame: frame)
    }
    
    required init?(coder aDecoder: NSCoder) {
        super.init(coder: aDecoder)
        setupBreadcrumb()
    }
    
    
    func setupBreadcrumb() {
        Bundle.main.loadNibNamed("BiometricBreadcrumb", owner: self, options: nil)
        contentView.bounds = self.bounds
        contentView.autoresizingMask = [.flexibleHeight, .flexibleWidth]
        
        breadcrumbButton.accessibilityLabel = NSLocalizedString("MyAccountTitle", comment: "")
        breadcrumbButton.accessibilityHint = NSLocalizedString("MyAccountHint", comment: "")
        
        self.biometricLabel.font = UIFont.preferredFont(forTextStyle: .body)
        self.biometricLabel.text = NSLocalizedString("BreadcrumbMessage", comment: "")
        
        self.biometricLabel.adjustsFontForContentSizeCategory = true
        
        // specifies the elements in the header which should be read out when using talkback (and the order which they're read out)
        contentView.accessibilityElements = [biometricLabel]
        addSubview(contentView)
    }
    
}

