
import UIKit

class HeaderBar: UIView {
    @IBOutlet var headerBarView: UIView!
    @IBOutlet weak var NHSHomeLogo: UIImageView!
    @IBOutlet weak var helpIcon: UIButton!
    @IBOutlet weak var moreIcon: UIButton!

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
        moreIcon.accessibilityLabel = NSLocalizedString("MoreTitle", comment: "")
        moreIcon.accessibilityHint = NSLocalizedString("MoreHint", comment: "")
        helpIcon.accessibilityLabel = NSLocalizedString("HelpTitle", comment: "")
        helpIcon.accessibilityHint = NSLocalizedString("HelpHint", comment: "")
        NHSHomeLogo.accessibilityLabel = NSLocalizedString("NHSAppHomeTitle", comment: "")
        NHSHomeLogo.accessibilityHint = NSLocalizedString("NHSAppHomeHint", comment: "")
        // specifies the elements in the header which should be read out when using talkback (and the order which they're read out)
        headerBarView.accessibilityElements = [NHSHomeLogo!, helpIcon!, moreIcon!]
        addSubview(headerBarView)
    }
    
    func addBottomShadow() {
        self.layer.masksToBounds = false
        self.layer.shadowColor = UIColor.black.cgColor
        self.layer.shadowOffset = CGSize(width: -1, height: 1)
        self.layer.shadowRadius = 5
        self.layer.shadowOpacity = 0.5
    }
}
