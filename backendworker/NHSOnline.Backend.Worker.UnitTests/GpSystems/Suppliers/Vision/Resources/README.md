Added mycert.pfx which is used within the envelope service tests. This was needed as the certificate needs setup with a private key
and the easiest way to complete this was to add a dummy certificate which is not the same as the one used in production.

This file is copied to the output directory and then read in during the setup of the test.