function create_certificate () {
  local CERTIFICATE_PATH="${1}"
  local CERTIFCATE_COMMON_NAME="${2}"
  local CERTIFCATE_DISTINGUISHED_NAME="${3}"

  mkdir -p $CERTIFICATE_PATH

  echo Creating config

  cat > $CERTIFICATE_PATH/https.config  << EOF
[ req ]
default_bits = 2048
default_md = sha256
default_keyfile = key.pem
prompt = no
encrypt_key = no

distinguished_name = ${CERTIFCATE_DISTINGUISHED_NAME}
req_extensions = v3_req
x509_extensions = v3_req

[ ${CERTIFCATE_DISTINGUISHED_NAME} ]
commonName = "${CERTIFCATE_COMMON_NAME}"

[ v3_req ]
subjectAltName = @alt_names
basicConstraints = critical, CA:true
keyUsage = critical, keyEncipherment
extendedKeyUsage = critical, 1.3.6.1.5.5.7.3.1

[alt_names]
DNS.1      = local.bitraft.io
DNS.2      = securestubs.local.bitraft.io
DNS.3      = pkb.securestubs.local.bitraft.io
DNS.4      = securestubs.local.bitraft.io
DNS.5      = android.local.bitraft.io

[ CA_default ]
copy_extensions = copy
EOF

  echo Creating certificate signing request
  (cd $CERTIFICATE_PATH  && openssl req -config https.config -new -out csr.pem)

  echo Creating self-signed certificate
  (cd $CERTIFICATE_PATH && openssl x509 -req -days 365 -extfile https.config -extensions v3_req -in csr.pem -signkey key.pem -out ${CERTIFCATE_DISTINGUISHED_NAME}.crt)

  echo Generating PFX
  (cd $CERTIFICATE_PATH && openssl pkcs12 -export -out ${CERTIFCATE_DISTINGUISHED_NAME}.pfx -inkey key.pem -in ${CERTIFCATE_DISTINGUISHED_NAME}.crt -password pass:)

  echo Certificate Created
}
