#! /bin/bash

set -e

CERTS=0
function copy_cert {
    if [ -f "$1" ]; then
        cp "$1" /usr/local/share/ca-certificates/`basename "$1"`
        CERTS=$((CERTS+1))
    fi
}

copy_cert Suppliers/Tpp/PublicKeys/TppDev.crt
copy_cert Suppliers/Tpp/PublicKeys/TppProd.crt
copy_cert Suppliers/Vision/PublicKeys/vision-test-env.crt
copy_cert Suppliers/Spine/PublicKeys/root-dev.crt
copy_cert Suppliers/Spine/PublicKeys/subca-dev.crt
copy_cert Suppliers/Spine/PublicKeys/spine-dev.crt
copy_cert Suppliers/Spine/PublicKeys/queue-staging.nhsapp.service.nhs.uk.crt
copy_cert Suppliers/Spine/PublicKeys/root-staging.crt
copy_cert Suppliers/Spine/PublicKeys/subca-staging.crt
copy_cert Suppliers/Spine/PublicKeys/nhsapp.ncrs.nhs.uk.crt
copy_cert Suppliers/Spine/PublicKeys/subca-prod.crt
copy_cert Suppliers/Spine/PublicKeys/root-prod.crt

chmod 755 /usr/local/share/ca-certificates/
find /usr/local/share/ca-certificates/ -type f -print -exec chmod 644 {} \;
update-ca-certificates

# Disable TLS 1.3 for EMIS connectivity
sed -i "s|DEFAULT@SECLEVEL=2|DEFAULT@SECLEVEL=1|g" /etc/ssl/openssl.cnf
