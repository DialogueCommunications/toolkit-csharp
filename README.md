# toolkit-csharp

## About

A C# client library to send SMS messages through the Dialogue SMS Toolkit API: http://www.dialogue.net/sms_toolkit/

## Build

    git clone git://github.com/DialogueCommunications/toolkit-csharp.git

Open toolkit-csharp/Dialogue Partner Toolkit Client Library.sln using Visual C# 2010 Express or Visual Studio 2010.

## Example Usage

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    
    using DialoguePartnerToolkit.Sms;
    
    namespace ConsoleApplication1
    {
        class Program
        {
            static void Main(string[] args)
            {
                SendSmsClient client = new SendSmsClient()
                {
                    Credentials = new System.Net.NetworkCredential("user", "pass"),
                    Secure = true
                };
    
                SendSmsRequest request = new SendSmsRequest()
                {
                    Messages = new List<string> { "This is a test message" },
                    Recipients = new List<string> { "44xxxxxxxxxx" }
                };
    
                SendSmsResponse response = client.SendSms(request);
            }
        }
    }

## References

* [C# Quick Start Guide][quick_start_guide]

 [quick_start_guide]: http://www.dialogue.net/sms_toolkit/documents/Dialogue_Partner_Toolkit_Quick_Start_Guide_Csharp.pdf

## Contribute

If you would like to contribute to toolkit-csharp then fork the project, implement your feature/plugin on a new branch and send a pull request.
