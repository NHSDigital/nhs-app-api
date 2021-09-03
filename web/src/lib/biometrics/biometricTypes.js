const None = 'none';
const FaceID = 'face';
const TouchID = 'touch';
const FingerprintFaceOrIris = 'fingerPrintFaceOrIris';

// Will be removed once Xamarin is released
// we do not support Face or Iris on legacy Android
const Fingerprint = 'fingerPrint';

export default {
  None,
  FaceID,
  TouchID,
  Fingerprint,
  FingerprintFaceOrIris,
};
