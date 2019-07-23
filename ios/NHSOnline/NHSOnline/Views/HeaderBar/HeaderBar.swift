
import UIKit

class HeaderBar: UIView {
    @IBOutlet var headerBarView: UIView!
    @IBOutlet weak var headerTitle: UILabel!
    @IBOutlet weak var NHSHomeLogo: UIImageView!
    @IBOutlet weak var homeLogo: UIImageView!
    @IBOutlet weak var helpIcon: UIButton!
    @IBOutlet weak var myAccountIcon: UIButton!

    override init(frame: CGRect) {
        super.init(frame: frame)
    }
    
    required init?(coder aDecoder: NSCoder) {
        super.init(coder: aDecoder)
        setupHeaderBar()
    }
    
    func setupHeaderBar() {
        Bundle.main.loadNibNamed("HeaderBar", owner: self, options: nil)
        headerBarView.bounds = self.bounds
        headerBarView.autoresizingMask = [.flexibleHeight, .flexibleWidth]
        myAccountIcon.accessibilityLabel = NSLocalizedString("MyAccountTitle", comment: "")
        myAccountIcon.accessibilityHint = NSLocalizedString("MyAccountHint", comment: "")
        helpIcon.accessibilityLabel = NSLocalizedString("HelpTitle", comment: "")
        helpIcon.accessibilityHint = NSLocalizedString("HelpHint", comment: "")
        NHSHomeLogo.accessibilityLabel = NSLocalizedString("NHSAppHomeTitle", comment: "")
        NHSHomeLogo.accessibilityHint = NSLocalizedString("NHSAppHomeHint", comment: "")
        self.headerTitle.font = UIFont.preferredFont(forTextStyle: .headline)
        if #available(iOS 10.0, *) {
            self.headerTitle.adjustsFontForContentSizeCategory = true
        }
        // specifies the elements in the header which should be read out when using talkback (and the order which they're read out)
        headerBarView.accessibilityElements = [NHSHomeLogo, helpIcon, myAccountIcon, headerTitle]
        addSubview(headerBarView)
    }
    
    func addBottomShadow() {
        self.layer.masksToBounds = false
        self.layer.shadowColor = UIColor.black.cgColor
        self.layer.shadowOffset = CGSize(width: -1, height: 1)
        self.layer.shadowRadius = 5
        self.layer.shadowOpacity = 0.5
    }
    
    func setFocusToNhsLogoForA11y() {
        if !isHidden && UIAccessibilityIsVoiceOverRunning() {
            UIAccessibilityPostNotification(UIAccessibilityLayoutChangedNotification, self.NHSHomeLogo)
        }
    }
}
