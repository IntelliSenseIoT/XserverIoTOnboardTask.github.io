# Troubleshouting

## Cloning the project with Visual Studio 2019 - No certificate found with the supplied thumbprint

    Solution:
    
    Right click the project -> Properties -> Package Manifest
    On the Package.appxmanifest go to Packaging tab -> Choose Certificate
    In the new window click "Select a Certificate..." if you have one, or create a certificate if you haven't created one

[More details](https://stackoverflow.com/questions/57578299/uwp-no-certificate-found-with-the-supplied-thumbprint)
