$cer = New-SelfSignedCertificate -Subject "VPN Buddy" -CertStoreLocation Cert:\CurrentUser\My -KeyProtection Protect
$pass = Read-Host -Prompt 'Input the password for the certificate' -AsSecureString
Export-PfxCertificate -Cert $cer -FilePath "key.pfx" -Password $pass