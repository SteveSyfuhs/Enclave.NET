#!pwsh.exe

if ($IsCore -and (-not $IsWindows)) {
    Write-Error "Sorry, this script only supports Windows for now."
    return
}


$storeLocation = "CurrentUser"
$storeName = "My"
$certStoreLocation = "cert:\$storeLocation\$storeName"


$server = New-SelfSignedCertificate -CertStoreLocation $certStoreLocation -DnsName server.local
$client = New-SelfSignedCertificate -CertStoreLocation $certStoreLocation -DnsName client.local

$config = [ordered]@{
    Server = [ordered]@{
        ServerCertificate = [ordered]@{
            StoreLocation = $storeLocation
            StoreName     = $storeName
            Thumbprint    = $server.Thumbprint
        }
        ClientCertificates = @(
            [ordered]@{
                Thumbprint    = $client.Thumbprint
            }
        )
    }
}

$config | ConvertTo-Json -Depth 10 > $PSScriptRoot\settings.local.json
