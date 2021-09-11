# c-sharp-collection
Just a small collection of C# programs that I haven't found a better place for yet.

## ClientWebSocket.cs
It's incredible cumbersome to find ressources on writing a simple, native WebSocket client (and using JSON in C#).
This program can be used as a basis for an implant (stager) using WebSockets to connect to the C2.


## InvokeReflectiveDll.cs
Leverages (fileless) reflective DLL injection using C#.
Loads an external DLL into memory and executes a specific function.
> If you intend on using this for malicious purposes, you need to implement an AMSI bypass, since with modern versions of .NET AMSI also checks _Assembly.Load_ 
