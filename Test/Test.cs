using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

using DialoguePartnerToolkit.Sms;

namespace Test
{
    class Test
    {
        delegate void TestDeletegate();

        private static void assertException(TestDeletegate testDelegate, Type type, string message)
        {
            try
            {
                testDelegate.Invoke();
                Debug.Assert(false, "Exception expected");
            }
            catch (Exception e)
            {
                Debug.Assert(e.GetType() == type);
                Debug.Assert(e.Message.Contains(message));
            }
        }

        static void Main(string[] args)
        {
            //
            // Credentials (unlike in Java and PHP, in C# we use System.Net.NetworkCredential, 
            // which allows empty values, so validation is done inside the SendSmsClient constructor)
            //

            // UserName=null
            assertException(delegate()
            {
                new SendSmsClient("endpoint", new NetworkCredential() { UserName = null, Password = "pass" });
            }, typeof(ArgumentNullException), "No UserName provided.");

            // UserName=""
            assertException(delegate()
            {
                new SendSmsClient("endpoint", new NetworkCredential() { UserName = "", Password = "pass" });
            }, typeof(ArgumentNullException), "No UserName provided.");

            // Password=null
            assertException(delegate()
            {
                new SendSmsClient("endpoint", new NetworkCredential() { UserName = "user", Password = null });
            }, typeof(ArgumentNullException), "No Password provided.");

            // Password=""
            assertException(delegate()
            {
                new SendSmsClient("endpoint", new NetworkCredential() { UserName = "user", Password = "" });
            }, typeof(ArgumentNullException), "No Password provided.");

            //
            // SmsSendRequest constructors
            //

            // message=(string)null
            assertException(delegate()
            {
                new SendSmsRequest((string)null, "recipient");
            }, typeof(ArgumentNullException), "No message provided.");

            // message=""
            assertException(delegate()
            {
                new SendSmsRequest("", "recipient");
            }, typeof(ArgumentNullException), "No message provided.");

            // message=(List<string>)null
            assertException(delegate()
            {
                new SendSmsRequest((List<string>)null, "recipient");
            }, typeof(ArgumentNullException), "No messages provided.");

            // message=<empty list>
            assertException(delegate()
            {
                new SendSmsRequest(new List<string>(), "recipient");
            }, typeof(ArgumentNullException), "No messages provided.");

            // recipient=(string)null
            assertException(delegate()
            {
                new SendSmsRequest("message", (string)null);
            }, typeof(ArgumentNullException), "No recipient provided.");

            // recipient=""
            assertException(delegate()
            {
                new SendSmsRequest("message", "");
            }, typeof(ArgumentNullException), "No recipient provided.");

            // recipient=(List<string>)null
            assertException(delegate()
            {
                new SendSmsRequest("message", (List<string>)null);
            }, typeof(ArgumentNullException), "No recipients provided.");

            // recipient=<empty list>
            assertException(delegate()
            {
                new SendSmsRequest("message", new List<string>());
            }, typeof(ArgumentNullException), "No recipients provided.");

            //
            // SmsSendRequest constructor setters
            //

            SendSmsRequest request = new SendSmsRequest("message", "recipient");

            // message=null
            assertException(delegate()
            {
                request.Message = null;
            }, typeof(ArgumentNullException), "No message provided.");

            // message=""
            assertException(delegate()
            {
                request.Message = "";
            }, typeof(ArgumentNullException), "No message provided.");

            // messages=null
            assertException(delegate()
            {
                request.Messages = null;
            }, typeof(ArgumentNullException), "No messages provided.");

            // messages=<empty list>
            assertException(delegate()
            {
                request.Messages = new List<string>();
            }, typeof(ArgumentNullException), "No messages provided.");

            // recipient=null
            assertException(delegate()
            {
                request.Recipient = null;
            }, typeof(ArgumentNullException), "No recipient provided.");

            // recipient=""
            assertException(delegate()
            {
                request.Recipient = "";
            }, typeof(ArgumentNullException), "No recipient provided.");

            // recipients=null
            assertException(delegate()
            {
                request.Recipients = null;
            }, typeof(ArgumentNullException), "No recipients provided.");

            // recipients=<empty list>
            assertException(delegate()
            {
                request.Recipients = new List<string>();
            }, typeof(ArgumentNullException), "No recipients provided.");

            //
            // SmsSendRequest constructor getters
            //

            request = new SendSmsRequest("message", "recipient");
            Debug.Assert(Enumerable.SequenceEqual(request.Messages, new List<string>() { "message" }));
            Debug.Assert(Enumerable.SequenceEqual(request.Recipients, new List<string>() { "recipient" }));
            request.Message = "message2";
            request.Recipient = "recipient2";
            Debug.Assert(Enumerable.SequenceEqual(request.Messages, new List<string>() { "message2" }));
            Debug.Assert(Enumerable.SequenceEqual(request.Recipients, new List<string>() { "recipient2" }));

            request = new SendSmsRequest(new List<string>() { "message", "message2" }, new List<string>() { "recipient", "recipient2" });
            Debug.Assert(Enumerable.SequenceEqual(request.Messages, new List<string>() { "message", "message2" }));
            Debug.Assert(Enumerable.SequenceEqual(request.Recipients, new List<string>() { "recipient", "recipient2" }));
            request.Messages = new List<string>() { "message", "message2", "message3" };
            request.Recipients = new List<string>() { "recipient", "recipient2", "recipient3" };
            Debug.Assert(Enumerable.SequenceEqual(request.Messages, new List<string>() { "message", "message2", "message3" }));
            Debug.Assert(Enumerable.SequenceEqual(request.Recipients, new List<string>() { "recipient", "recipient2", "recipient3" }));

            //
            // SmsSendRequest other getters/setters
            //

            request = new SendSmsRequest("message", "recipient");

            // Sender
            Debug.Assert(request.Sender == null);
            request.Sender = "Sender";
            Debug.Assert(request.Sender == "Sender");
            Debug.Assert(request["X-E3-Originating-Address"] == "Sender");

            request.Sender = null;
            Debug.Assert(request.Sender == null);
            Debug.Assert(!request.ContainsKey("X-E3-Originating-Address"));

            // ConcatenationLimit
            Debug.Assert(request.ConcatenationLimit == null);
            request.ConcatenationLimit = 255;
            Debug.Assert(request.ConcatenationLimit == 255);
            Debug.Assert(request["X-E3-Concatenation-Limit"] == "255");

            request.ConcatenationLimit = null;
            Debug.Assert(request.ConcatenationLimit == null);
            Debug.Assert(!request.ContainsKey("X-E3-Concatenation-Limit"));

            // ScheduleFor
            Debug.Assert(request.ScheduleFor == null);

            // Europe/London: 01/09/2012 12:30:00 -> UTC: 01/09/2012 11:30:00
            request.ScheduleFor = new DateTime(2012, 9, 1, 11, 30, 0, 0, DateTimeKind.Utc);
            Debug.Assert(request.ScheduleFor.Equals(new DateTime(2012, 9, 1, 11, 30, 0, 0, DateTimeKind.Utc)));
            Debug.Assert(request["X-E3-Schedule-For"] == "20120901123000");

            // Europe/Berlin: 01/09/2012 12:30:00 -> UTC: 01/09/2012 10:30:00
            request.ScheduleFor = new DateTime(2012, 9, 1, 10, 30, 0, 0, DateTimeKind.Utc);
            Debug.Assert(request.ScheduleFor.Equals(new DateTime(2012, 9, 1, 10, 30, 0, 0, DateTimeKind.Utc)));
            Debug.Assert(request["X-E3-Schedule-For"] == "20120901113000");

            // GMT: 01/09/2012 12:30:00 -> UTC: 01/09/2012 12:30:00
            request.ScheduleFor = new DateTime(2012, 9, 1, 12, 30, 0, 0, DateTimeKind.Utc);
            Debug.Assert(request.ScheduleFor.Equals(new DateTime(2012, 9, 1, 12, 30, 0, 0, DateTimeKind.Utc)));
            Debug.Assert(request["X-E3-Schedule-For"] == "20120901133000");

            request.ScheduleFor = null;
            Debug.Assert(request.ScheduleFor == null);
            Debug.Assert(!request.ContainsKey("X-E3-Schedule-For"));

            // ConfirmDelivery
            Debug.Assert(request.ConfirmDelivery == null);
            request.ConfirmDelivery = true;
            Debug.Assert(request.ConfirmDelivery == true);
            Debug.Assert(request["X-E3-Confirm-Delivery"] == "on");

            request.ConfirmDelivery = false;
            Debug.Assert(request.ConfirmDelivery == false);
            Debug.Assert(request["X-E3-Confirm-Delivery"] == "off");

            request.ConfirmDelivery = null;
            Debug.Assert(request.ConfirmDelivery == null);
            Debug.Assert(!request.ContainsKey("X-E3-Confirm-Delivery"));

            // ReplyPath
            Debug.Assert(request.ReplyPath == null);
            request.ReplyPath = "http://www.server.com/path";
            Debug.Assert(request.ReplyPath == "http://www.server.com/path");
            Debug.Assert(request["X-E3-Reply-Path"] == "http://www.server.com/path");

            request.ReplyPath = null;
            Debug.Assert(request.ReplyPath == null);
            Debug.Assert(!request.ContainsKey("X-E3-Reply-Path"));

            // UserKey
            Debug.Assert(request.UserKey == null);
            request.UserKey = "123457890";
            Debug.Assert(request.UserKey == "123457890");
            Debug.Assert(request["X-E3-User-Key"] == "123457890");

            request.UserKey = null;
            Debug.Assert(request.UserKey == null);
            Debug.Assert(!request.ContainsKey("X-E3-User-Key"));

            // SessionReplyPath
            Debug.Assert(request.SessionReplyPath == null);
            request.SessionReplyPath = "http://www.server.com/path";
            Debug.Assert(request.SessionReplyPath == "http://www.server.com/path");
            Debug.Assert(request["X-E3-Session-Reply-Path"] == "http://www.server.com/path");

            request.SessionReplyPath = null;
            Debug.Assert(request.SessionReplyPath == null);
            Debug.Assert(!request.ContainsKey("X-E3-Session-Reply-Path"));

            // SessionId
            Debug.Assert(request.SessionId == null);
            request.SessionId = "123457890";
            Debug.Assert(request.SessionId == "123457890");
            Debug.Assert(request["X-E3-Session-ID"] == "123457890");

            request.SessionId = null;
            Debug.Assert(request.SessionId == null);
            Debug.Assert(!request.ContainsKey("X-E3-Session-ID"));

            // UserTag
            Debug.Assert(request.UserTag == null);
            request.UserTag = "123457890";
            Debug.Assert(request.UserTag == "123457890");
            Debug.Assert(request["X-E3-User-Tag"] == "123457890");

            request.UserTag = null;
            Debug.Assert(request.UserTag == null);
            Debug.Assert(!request.ContainsKey("X-E3-User-Tag"));

            // ValidityPeriod
            Debug.Assert(request.ValidityPeriod == null);

            request.ValidityPeriod = new TimeSpan(14, 0, 0, 0);
            Debug.Assert(request.ValidityPeriod.Equals(new TimeSpan(14, 0, 0, 0)));
            Debug.Assert(request["X-E3-Validity-Period"] == "2w");

            request.ValidityPeriod = new TimeSpan(7, 0, 0, 0);
            Debug.Assert(request.ValidityPeriod.Equals(new TimeSpan(7, 0, 0, 0)));
            Debug.Assert(request["X-E3-Validity-Period"] == "1w");

            request.ValidityPeriod = new TimeSpan(2, 0, 0, 0);
            Debug.Assert(request.ValidityPeriod.Equals(new TimeSpan(2, 0, 0, 0)));
            Debug.Assert(request["X-E3-Validity-Period"] == "2d");

            request.ValidityPeriod = new TimeSpan(24, 0, 0);
            Debug.Assert(request.ValidityPeriod.Equals(new TimeSpan(24, 0, 0)));
            Debug.Assert(request["X-E3-Validity-Period"] == "1d");

            request.ValidityPeriod = new TimeSpan(2, 0, 0);
            Debug.Assert(request.ValidityPeriod.Equals(new TimeSpan(2, 0, 0)));
            Debug.Assert(request["X-E3-Validity-Period"] == "2h");

            request.ValidityPeriod = new TimeSpan(0, 60, 0);
            Debug.Assert(request.ValidityPeriod.Equals(new TimeSpan(0, 60, 0)));
            Debug.Assert(request["X-E3-Validity-Period"] == "1h");

            request.ValidityPeriod = new TimeSpan(0, 2, 0);
            Debug.Assert(request.ValidityPeriod.Equals(new TimeSpan(0, 2, 0)));
            Debug.Assert(request["X-E3-Validity-Period"] == "2m");

            request.ValidityPeriod = new TimeSpan(0, 0, 1);
            Debug.Assert(request.ValidityPeriod.Equals(new TimeSpan(0, 0, 0)));
            Debug.Assert(request["X-E3-Validity-Period"] == "0m");

            request.ValidityPeriod = null;
            Debug.Assert(request.ValidityPeriod == null);
            Debug.Assert(!request.ContainsKey("X-E3-Validity-Period"));

            // Custom properties
            request["X-E3-Custom-Property"] = "test1234";
            Debug.Assert(request["X-E3-Custom-Property"] == "test1234");
            request.Remove("X-E3-Custom-Property");
            Debug.Assert(!request.ContainsKey("X-E3-Custom-Property"));

            //
            // toString() (XML generation)
            //

            request = new SendSmsRequest(
                new List<string>() { "message", "message2" },
                new List<string>() { "recipient", "recipient2" }
            );
            request.Sender = "sender";
            request.ConcatenationLimit = 255;
            request.ScheduleFor = new DateTime(2012, 9, 1, 11, 30, 0, 0, DateTimeKind.Utc);
            request.ConfirmDelivery = true;
            request.ReplyPath = "http://www.server.com/path";
            request.UserKey = "123457890";
            request.SessionReplyPath = "http://www.server.com/path";
            request.SessionId = "1234567890";
            request.UserTag = "123457890";
            request.ValidityPeriod = new TimeSpan(14, 0, 0, 0);

            var sw = new StringWriter();
            var xw = XmlWriter.Create(sw);
            xw.WriteStartDocument();
            xw.WriteStartElement("sendSmsRequest");
            request.WriteXml(xw);
            xw.WriteEndElement();
            xw.WriteEndDocument();
            xw.Flush();

            Debug.Assert(sw.ToString() ==
                "<?xml version=\"1.0\" encoding=\"utf-16\"?><sendSmsRequest><X-E3-Message>message</X-E3-Message><X-E3-Message>message2</X-E3-Message><X-E3-Recipients>recipient</X-E3-Recipients><X-E3-Recipients>recipient2</X-E3-Recipients><X-E3-Originating-Address>sender</X-E3-Originating-Address><X-E3-Concatenation-Limit>255</X-E3-Concatenation-Limit><X-E3-Schedule-For>20120901123000</X-E3-Schedule-For><X-E3-Confirm-Delivery>on</X-E3-Confirm-Delivery><X-E3-Reply-Path>http://www.server.com/path</X-E3-Reply-Path><X-E3-User-Key>123457890</X-E3-User-Key><X-E3-Session-Reply-Path>http://www.server.com/path</X-E3-Session-Reply-Path><X-E3-Session-ID>1234567890</X-E3-Session-ID><X-E3-User-Tag>123457890</X-E3-User-Tag><X-E3-Validity-Period>2w</X-E3-Validity-Period></sendSmsRequest>"
            );

            //
            // SendSmsClient constructor
            //

            var credentials = new NetworkCredential()
            {
                UserName = "user",
                Password = "pass"
            };

            // endpoint=null
            assertException(delegate()
            {
                new SendSmsClient(null, credentials);
            }, typeof(ArgumentNullException), "No Endpoint provided.");

            // endpoint=""
            assertException(delegate()
            {
                new SendSmsClient("", credentials);
            }, typeof(ArgumentNullException), "No Endpoint provided.");

            // credentials=null
            assertException(delegate()
            {
                new SendSmsClient("endpoint", null);
            }, typeof(ArgumentNullException), "No Credentials provided.");

            //
            // SendSmsClient constructor setters
            //

            SendSmsClient client;

            client = new SendSmsClient();

            client = new SendSmsClient("endpoint", 
                new NetworkCredential() { UserName = "user", Password = "pass" }
            );

            // endpoint=null
            assertException(delegate()
            {
                client.Endpoint = null;
            }, typeof(ArgumentNullException), "No Endpoint provided.");

            // endpoint=""
            assertException(delegate()
            {
                client.Endpoint = "";
            }, typeof(ArgumentNullException), "No Endpoint provided.");

            // credentials=null
            assertException(delegate()
            {
                client.Credentials = null;
            }, typeof(ArgumentNullException), "No Credentials provided.");

            //
            // SendSmsClient constructor getters
            //

            client = new SendSmsClient();
            Debug.Assert(client.Endpoint == null);
            Debug.Assert(client.Credentials == null);

            client = new SendSmsClient("endpoint", new NetworkCredential() { UserName = "user", Password = "pass" });

            Debug.Assert(client.Endpoint == "endpoint");
            client.Endpoint = "endpoint2";
            Debug.Assert(client.Endpoint == "endpoint2");

            Debug.Assert(client.Credentials.UserName == "user");
            Debug.Assert(client.Credentials.Password == "pass");
            client.Credentials = new NetworkCredential() { UserName = "user2", Password = "pass2" };
            Debug.Assert(client.Credentials.UserName == "user2");
            Debug.Assert(client.Credentials.Password == "pass2");

            //
            // SendSmsClient Path
            //

            client = new SendSmsClient("endpoint", new NetworkCredential() { UserName = "user", Password = "pass" });

            Debug.Assert(client.Path == "/submit_sm");

            // path=null
            assertException(delegate()
            {
                client.Path = null;
            }, typeof(ArgumentNullException), "No Path provided.");

            // path=""
            assertException(delegate()
            {
                client.Path = "";
            }, typeof(ArgumentNullException), "No Path provided.");

            // path invalid
            assertException(delegate()
            {
                client.Path = "path";
            }, typeof(ArgumentException), "The path must start with '/'.");

            client.Path = "/path";
            Debug.Assert(client.Path == "/path");

            //
            // SendSmsClient Secure
            //

            client = new SendSmsClient("endpoint", new NetworkCredential() { UserName = "user", Password = "pass" });

            Debug.Assert(client.Secure);
            client.Secure = false;
            Debug.Assert(!client.Secure);           

            //
            // SendSmsClient Submission
            //

            if (args.Length != 2 || String.IsNullOrEmpty(args[0]) || String.IsNullOrEmpty(args[1]))
            {
                Console.WriteLine("Usage: Test.exe <login> <password>");
                return;
            }

            string login = args[0];
            string password = args[1];

            try
            {
                client = new SendSmsClient("sms.dialogue.net", new NetworkCredential(login, password));

                client.Secure = false;
                testClient(client);
                client.Secure = true;                
                testClient(client);

                client = new SendSmsClient("sendmsg.dialogue.net", new NetworkCredential(login, password));

                client.Secure = false;
                testClient(client);
                client.Secure = true;                
                testClient(client);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Debug.Assert(false, e.ToString());
            }

            // Test sending uninitialized request
            assertException(delegate()
            {
                client.SendSms(new SendSmsRequest());
            }, typeof(InvalidOperationException), "SendSmsRequest has not been initialized.");                     

            // Test wrong password
            assertException(delegate()
            {
                testClient(new SendSmsClient("sendmsg.dialogue.net", new NetworkCredential("wrong", "wrong")));
            }, typeof(WebException), "The remote server returned an error: (401) Unauthorized.");

            // Test wrong endpoint
            assertException(delegate()
            {
                testClient(new SendSmsClient("wrong", new NetworkCredential("wrong", "wrong")));
            }, typeof(WebException), "The remote name could not be resolved:");

            //
            // SmsReport
            //

            try
            {
                SmsReport report = SmsReport.getInstance("<callback X-E3-Delivery-Report=\"20\" X-E3-ID=\"90A9893BC2B645918034F4C358A062CE\" X-E3-Loop=\"1322229741.93646\" X-E3-Network=\"Orange\" X-E3-Recipients=\"447xxxxxxxxx\" X-E3-Timestamp=\"2011-12-01 18:02:21\" X-E3-User-Key=\"myKey1234\"/>");

                Debug.Assert(report != null);
                Debug.Assert(report.Id == "90A9893BC2B645918034F4C358A062CE");
                Debug.Assert(report.Recipient == "447xxxxxxxxx");
                Debug.Assert(report.DeliveryReport == "20");
                Debug.Assert(report.UserKey == "myKey1234");
                Debug.Assert(report.Timestamp.Ticks == 634583593410000000);
                Debug.Assert(report.Network == "Orange");
                Debug.Assert(report.ToString() == "Id: 90A9893BC2B645918034F4C358A062CE, Recipient: 447xxxxxxxxx, DeliveryReport: 20, UserKey: myKey1234, Timestamp: 01/12/2011 18:02:21, Network: Orange");

                report.DeliveryReport = "00";
                Debug.Assert(report.State == State.Delivered);
                Debug.Assert(report.Successful);
                report.DeliveryReport = "1F";
                Debug.Assert(report.State == State.Delivered);
                Debug.Assert(report.Successful);
                report.DeliveryReport = "20";
                Debug.Assert(report.State == State.TemporaryError);
                Debug.Assert(!report.Successful);
                report.DeliveryReport = "3F";
                Debug.Assert(report.State == State.TemporaryError);
                Debug.Assert(!report.Successful);
                report.DeliveryReport = "40";
                Debug.Assert(report.State == State.PermanentError);
                Debug.Assert(!report.Successful);
                report.DeliveryReport = "7F";
                Debug.Assert(report.State == State.PermanentError);
                Debug.Assert(!report.Successful);

                report.DeliveryReport = "80";
                assertException(delegate()
                {
                    var state = report.State;
                }, typeof(InvalidOperationException), "Unknown delivery report value:");

                report.DeliveryReport = "";
                Debug.Assert(report.State == State.Undefined);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Debug.Assert(false, e.ToString());
            }

            //
            // SmsReply
            //

            try
            {
                SmsReply reply = SmsReply.getInstance("<callback X-E3-Account-Name=\"test\" X-E3-Data-Coding-Scheme=\"00\" X-E3-Hex-Message=\"54657374204D657373616765\" X-E3-ID=\"809EF683F022441DB9C4895AED6382CF\" X-E3-Loop=\"1322223264.20603\" X-E3-MO-Campaign=\"\" X-E3-MO-Keyword=\"\" X-E3-Network=\"Orange\" X-E3-Originating-Address=\"447xxxxxxxxx\" X-E3-Protocol-Identifier=\"00\" X-E3-Recipients=\"1234567890\" X-E3-Session-ID=\"1234567890\" X-E3-Timestamp=\"2011-11-25 12:14:23.000000\" X-E3-User-Data-Header-Indicator=\"0\"/>");

                Debug.Assert(reply != null);
                Debug.Assert(reply.Id == "809EF683F022441DB9C4895AED6382CF");
                Debug.Assert(reply.Sender == "447xxxxxxxxx");
                Debug.Assert(reply.SessionId == "1234567890");
                Debug.Assert(reply.HexMessage == "54657374204D657373616765");
                Debug.Assert(reply.Message == "Test Message");
                Debug.Assert(reply.Timestamp.Ticks == 634578200630000000);
                Debug.Assert(reply.Network == "Orange");
                Debug.Assert(reply.ToString() == "Id: 809EF683F022441DB9C4895AED6382CF, Sender: 447xxxxxxxxx, SessionId: 1234567890, HexMessage: 54657374204D657373616765, Message: Test Message, Timestamp: 25/11/2011 12:14:23, Network: Orange");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Debug.Assert(false, e.ToString());
            }
        }

        private static void testClient(SendSmsClient client)
        {
            SendSmsRequest request = new SendSmsRequest(
                "This is a test message.", new List<string>() { "447956247525", "34637975280", "999" });

            SendSmsResponse response = client.SendSms(request);

            Debug.Assert(response != null && response.Messages != null && response.Messages.Count() == 3);

            Sms sms;

            sms = response.Messages[0];
            Debug.Assert(sms.Successful);
            Debug.Assert(sms.Recipient == "447956247525");
            Debug.Assert(!String.IsNullOrEmpty(sms.Id));
            Debug.Assert(sms.SubmissionReport == "00");
            Debug.Assert(String.IsNullOrEmpty(sms.ErrorDescription));

            sms = response.Messages[1];
            Debug.Assert(sms.Successful);
            Debug.Assert(sms.Recipient == "34637975280");
            Debug.Assert(!String.IsNullOrEmpty(sms.Id));
            Debug.Assert(sms.SubmissionReport == "00");
            Debug.Assert(String.IsNullOrEmpty(sms.ErrorDescription));

            sms = response.Messages[2];
            Debug.Assert(!sms.Successful);
            Debug.Assert(sms.Recipient == "999");
            Debug.Assert(String.IsNullOrEmpty(sms.Id));
            Debug.Assert(sms.SubmissionReport == "43");
            Debug.Assert(!String.IsNullOrEmpty(sms.ErrorDescription));
        }
    }
}
