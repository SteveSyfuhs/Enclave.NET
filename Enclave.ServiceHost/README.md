# PKI Certificates (X.509)

To run the ServiceHost and many of the samples, you'll need configure a pair of PKI certificates,
one for the server and one for the client, so that they can securely communicate with each other.

On Windows, you can use the included `Generate-TestCertificates.ps1` script to generate new
self-signed X.509 certificates and install them into the Windows certificate store scoped to the
current user.  The script will also generate a *local* configuration settings file that will
reference the generates certificates when running the ServiceHost process and most of the samples.

## Non-Windows Systems

On non-Windows systems, you'll need to use the native facilities of that OS to manage
certificates in the local stores, and then reference the Thumbprints of the target
certs in the settings files.

Here's more info on X509Stores on non-Windows systems:

    * https://github.com/dotnet/corefx/issues/16879#issuecomment-285489189
