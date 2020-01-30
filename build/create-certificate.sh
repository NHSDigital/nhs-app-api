echo Creating Certificate

BASE_PATH=$HOME/.nhsonline/local-development-certificate

mkdir -p $BASE_PATH

echo Creating config

cat > $BASE_PATH/https.config  << EOF
[ req ]
default_bits = 2048
default_md = sha256
default_keyfile = key.pem
prompt = no
encrypt_key = no

distinguished_name = local-development-certificate
req_extensions = v3_req
x509_extensions = v3_req

[ local-development-certificate ]
commonName = "localhost"

[ v3_req ]
subjectAltName = @alt_names
basicConstraints = critical, CA:true
keyUsage = critical, keyEncipherment
extendedKeyUsage = critical, 1.3.6.1.5.5.7.3.1

[alt_names]
DNS.1      = local.bitraft.io
DNS.2      = android.local.bitraft.io
DNS.3      = localhost

[ CA_default ]
copy_extensions = copy
EOF

echo Creating certificate signing request
(cd $BASE_PATH && openssl req -config https.config -new -out csr.pem)

echo Creating self-signed certificate
(cd $BASE_PATH && openssl x509 -req -days 365 -extfile https.config -extensions v3_req -in csr.pem -signkey key.pem -out local-development-https.crt)

echo Generating PFX
(cd $BASE_PATH && openssl pkcs12 -export -out local-development-https.pfx -inkey key.pem -in local-development-https.crt -password pass:)

echo Certificate Created


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
