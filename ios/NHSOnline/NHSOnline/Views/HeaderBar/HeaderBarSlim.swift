
import UIKit

class HeaderBarSlim: UIView {
    @IBOutlet var headerBarView: UIView!
    @IBOutlet weak var headerTitle: UILabel!
    @IBOutlet weak var backButtonArrow: UIButton!
    
    override init(frame: CGRect) {
        super.init(frame: frame)
    }
    
    required init?(coder aDecoder: NSCoder) {
        super.init(coder: aDecoder)
        setupHeaderBar()
    }
    
    func setupHeaderBar() {
        Bundle.main.loadNibNamed("HeaderBarSlim", owner: self, options: nil)
        headerBarView.bounds = self.bounds
        headerBarView.autoresizingMask = [.flexibleHeight, .flexibleWidth]
        backButtonArrow.accessibilityLabel = NSLocalizedString("BackArrowTitle", comment: "")
        backButtonArrow.accessibilityHint = NSLocalizedString("BackArrowHint", comment: "")
        self.headerTitle.font = UIFont.preferredFont(forTextStyle: .headline)
        if #available(iOS 10.0, *) {
            self.headerTitle.adjustsFontForContentSizeCategory = true
        }
        // specifies the elements in the header which should be read out when using talkback (and the order which they're read out)
        headerBarView.accessibilityElements = [backButtonArrow, headerTitle]
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
            UIAccessibilityPostNotification(UIAccessibilityLayoutChangedNotification, self.backButtonArrow)
        }
    }
}
