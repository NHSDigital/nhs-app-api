# shellcheck source=lib/functions.sh
source "buildscripts/lib/functions.sh"

create_certificate "$HOME/.nhsonline/local-development-certificate"  localhost secure-stubs-https

echo
echo The certificate should be imported as a trusted root certificate on your local machine and on any iOS/Android simulators that will be used:
echo
echo On Windows open up a powershell prompt as administrator and execute:
echo -    import-pfxcertificate -FilePath \$HOME\\.nhsonline\\local-development-certificate\\local-development-https.pfx cert:\\localMachine\\Root
echo
echo On Mac execute:
echo -    sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain \$HOME/.nhsonline/local-development-certificate/local-development-https.crt
echo
echo On Android Emulator:
echo -    Drag and drop \$HOME/.nhsonline/local-development-certificate/local-development-https.crt into the emulator
echo -    Navigate to Settings \> Security and Location \> Encryption and Credentials \> Install from SD Card
echo -    Select the certificate that was just copied to the phone \(in downloads\)
echo -    Give the certificate a name, say "LocalDevCert"
echo -    Follow the instructions to set a pin \(a fingerprint is not needed\)
echo
echo On iOS Simulator:
echo -    Drag and drop \$HOME/.nhsonline/local-development-certificate/local-development-https.crt into the emulator
echo -    Click to allow, then click to install the certificate
echo -    Navigate to Settings \> General \> About \> Certificate Trust Settings
echo -    Enable full trust for the newly installed localhost certificate by sliding the switch on the right
