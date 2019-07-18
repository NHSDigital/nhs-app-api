
import UIKit

class UnderlinedLabel: UILabel {
    
    override var text: String? {
        didSet {
            guard let text = text else { return }
            
            let boldAttrs = [NSAttributedStringKey.font : UIFont.boldSystemFont(ofSize: 15)]
            let attributedText = NSMutableAttributedString(string: text,
                                                           attributes: boldAttrs)
            attributedText.addAttribute(NSAttributedString.Key.underlineStyle ,
                                        value: NSUnderlineStyle.styleSingle.rawValue,
                                        range: NSMakeRange(0, text.count))
            self.attributedText = attributedText
        }
    }
}
