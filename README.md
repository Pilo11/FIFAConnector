# FIFAConnector
The FIFAConnector (for FIFA 11) allows you to establish a connection between two or multiple clients for a good old LAN game (f.e. with a Wiregaurd VPN)

# Create standalone EXE file

```
dotnet publish -c Release -r win-x86 -p:PublishSingleFile=true --self-contained true
```