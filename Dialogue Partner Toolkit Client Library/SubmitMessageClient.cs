using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;

namespace DialoguePartnerToolkit.Sms
{
    /// <summary>
    /// Request object containing message(s), recipient(s) and other optional properties.
    /// </summary>
    [XmlRoot("sendSmsRequest")]
    public class SendSmsRequest : Dictionary<string, string>, IXmlSerializable
    {
        private static TimeZoneInfo timeZoneInformation = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

        /// <summary>
        /// Constructs a SendSmsRequest object without message or recipient.
        /// </summary>
        public SendSmsRequest()
        {
        }

        /// <summary>
        /// Constructs a SendSmsRequest object with a single message and single recipient.
        /// </summary>
        /// <param name="Message">The message.</param>
        /// <param name="Recipient">The recipient.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when Message or Recipient is null or empty.</exception>
        public SendSmsRequest(string Message, string Recipient)
        {
            this.Message = Message;
            this.Recipient = Recipient;
        }

        /// <summary>
        /// Constructs a SendSmsRequest object with a single message and a list of recipients.
        /// </summary>
        /// <param name="Message">The message.</param>
        /// <param name="Recipients">The recipients list.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when Message or Recipients is null or empty.</exception>        
        public SendSmsRequest(string Message, List<string> Recipients)
        {
            this.Message = Message;
            this.Recipients = Recipients;
        }

        /// <summary>
        /// Constructs a SendSmsRequest object with a list of messages and single recipient.
        /// </summary>
        /// <param name="Messages">The messages list.</param>
        /// <param name="Recipient">The recipient.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when Messages or Recipient is null or empty.</exception>
        public SendSmsRequest(List<string> Messages, string Recipient)
        {
            this.Messages = Messages;
            this.Recipient = Recipient;
        }

        /// <summary>
        /// Constructs a SendSmsRequest object with a list of messages and a list of recipients.
        /// </summary>
        /// <param name="Messages">The messages list.</param>
        /// <param name="Recipients">The recipients list.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when Messages or Recipients is null or empty.</exception>
        public SendSmsRequest(List<string> Messages, List<string> Recipients)
        {
            this.Messages = Messages;
            this.Recipients = Recipients;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {            
            foreach (string message in Messages)
            {
                writer.WriteStartElement("X-E3-Message");
                writer.WriteValue(message);
                writer.WriteEndElement();
            }

            foreach (string recipient in Recipients)
            {
                writer.WriteStartElement("X-E3-Recipients");
                writer.WriteValue(recipient);
                writer.WriteEndElement();
            }

            foreach (string key in Keys)
            {
                writer.WriteStartElement(key);
                writer.WriteValue(this[key]);
                writer.WriteEndElement();
            }
        }

        private List<string> _Messages;

        /// <summary>
        /// The messages list.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown on setting when Messages is null or empty.</exception>
        public List<string> Messages
        {
            get
            {
                return _Messages;
            }

            set
            {
                if (value == null || value.Count() == 0)
                {
                    throw new ArgumentNullException("No messages provided.");
                }

                _Messages = value;
            }
        }

        /// <summary>
        /// Replaces the messages list by a single message. Setter only; to get the message(s) use property Messages.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown on setting when Message is null or empty.</exception>
        public string Message
        {
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("No message provided.");
                }

                Messages = new List<string> { value };
            }
        }

        private List<string> _Recipients;

        /// <summary>
        /// The recipients list.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown on setting when Recipients is null or empty.</exception>
        public List<string> Recipients
        {
            get
            {
                return _Recipients;
            }

            set
            {
                if (value == null || value.Count() == 0)
                {
                    throw new ArgumentNullException("No recipients provided.");
                }

                _Recipients = value;
            }
        }

        /// <summary>
        /// Replaces the recipients list by a single recipient. Setter only; to get the recipient(s) use property Recipients.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown on setting when Recipient is null or empty.</exception>
        public string Recipient
        {
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("No recipient provided.");
                }

                Recipients = new List<string> { value };
            }
        }

        // No documentation on purpose
        public string Sender
        {
            get
            {
                if (ContainsKey("X-E3-Originating-Address"))
                {
                    return this["X-E3-Originating-Address"];
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value != null)
                {
                    this["X-E3-Originating-Address"] = value;
                }
                else
                {
                    Remove("X-E3-Originating-Address");
                }
            }
        }

        /// <summary>
        /// By setting the ConcatenationLimit property, you enable long message concatenation. If the length of any message exceeds the default character limit the messaging gateway will send multiple concatenated messages up to the concatenation limit, after which the text is truncated. Concatenation works by splitting and wrapping the message in packets or fragments, each fragment being prefixed by the current fragment number and the total number of fragments. This allows the phone to know when it received all fragments and how to reassemble the message even if fragments arrive out of order. The concatenation limit refers to the maximum number of message fragments, not the number of characters, i.e. a concatenation limit of “3” means no more than 3 SMS messages will be sent, which in total can contain up to 459 GSM-compatible characters. The concatenation overhead is 7 characters, so 160 - 7 = 153 available characters per fragment x 3 = 459 total characters. In the response you will find one Sms object for each message segment.
        /// </summary>
        public int? ConcatenationLimit
        {
            get
            {
                if (ContainsKey("X-E3-Concatenation-Limit"))
                {
                    return int.Parse(this["X-E3-Concatenation-Limit"]);
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value.HasValue)
                {
                    this["X-E3-Concatenation-Limit"] = value.Value.ToString();
                }
                else
                {
                    Remove("X-E3-Concatenation-Limit");
                }
            }
        }

        /// <summary>
        /// Use the ScheduleFor property to delay sending messages until the specified date and time. The property is of type DateTime and must be an instance of DateTimeKind.Utc so that time zone conversion is possible. If the schedule date/time is in the past the message is sent immediately.
        /// </summary>
        public DateTime? ScheduleFor
        {
            get
            {
                if (ContainsKey("X-E3-Schedule-For"))
                {
                    DateTime dateTime = DateTime.ParseExact(this["X-E3-Schedule-For"], "yyyyMMddHHmmss", null);
                    return TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZoneInformation);
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value.HasValue)
                {
                    this["X-E3-Schedule-For"] = TimeZoneInfo.ConvertTimeFromUtc(
                        value.Value, timeZoneInformation).ToString("yyyyMMddHHmmss");
                }
                else
                {
                    Remove("X-E3-Schedule-For");
                }
            }
        }

        /// <summary>
        /// The property ConfirmDelivery can be set to true to enable tracking of message delivery. If you enable ConfirmDelivery you must also set the ReplyPath property pointing to an HTTP event handler that you implement. Optionally, set UserKey to an arbitrary, custom identifier, which will be posted back to you. You can use this to associate a message submission with a delivery report.
        /// </summary>
        /// <seealso cref="ReplyPath"/>
        /// <seealso cref="UserKey"/>
        public bool? ConfirmDelivery
        {
            get
            {
                if (ContainsKey("X-E3-Confirm-Delivery"))
                {
                    return this["X-E3-Confirm-Delivery"] == "on";
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value.HasValue)
                {
                    this["X-E3-Confirm-Delivery"] = value.Value ? "on" : "off";
                }
                else
                {
                    Remove("X-E3-Confirm-Delivery");
                }
            }
        }

        /// <see cref="ConfirmDelivery"/>
        public string ReplyPath
        {
            get
            {
                if (ContainsKey("X-E3-Reply-Path"))
                {
                    return this["X-E3-Reply-Path"];
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value != null)
                {
                    this["X-E3-Reply-Path"] = value;
                }
                else
                {
                    Remove("X-E3-Reply-Path");
                }
            }
        }

        /// <see cref="ConfirmDelivery"/>
        public string UserKey
        {
            get
            {
                if (ContainsKey("X-E3-User-Key"))
                {
                    return this["X-E3-User-Key"];
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value != null)
                {
                    this["X-E3-User-Key"] = value;
                }
                else
                {
                    Remove("X-E3-User-Key");
                }
            }
        }

        /// <summary>
        /// The property SessionReplyPath points to an HTTP event handler that you have implemented. The handler is invoked if the recipient replies to the message they have received. Optionally you can specify the SessionId property, which will be posted back to you. You can use this to associate a message submission with a reply.
        /// </summary>
        public string SessionReplyPath
        {
            get
            {
                if (ContainsKey("X-E3-Session-Reply-Path"))
                {
                    return this["X-E3-Session-Reply-Path"];
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value != null)
                {
                    this["X-E3-Session-Reply-Path"] = value;
                }
                else
                {
                    Remove("X-E3-Session-Reply-Path");
                }
            }
        }

        /// <see cref="SessionReplyPath"/>
        public string SessionId
        {
            get
            {
                if (ContainsKey("X-E3-Session-ID"))
                {
                    return this["X-E3-Session-ID"];
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value != null)
                {
                    this["X-E3-Session-ID"] = value;
                }
                else
                {
                    Remove("X-E3-Session-ID");
                }
            }
        }

        /// <summary>
        /// By setting the UserTag property you can tag messages. You can use this for billing purposes when sending messages on behalf of several customers. The UserTag property must not exceed 50 characters; longer values will make the submission fail.
        /// </summary>
        public string UserTag
        {
            get
            {
                if (ContainsKey("X-E3-User-Tag"))
                {
                    return this["X-E3-User-Tag"];
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value != null)
                {
                    this["X-E3-User-Tag"] = value;
                }
                else
                {
                    Remove("X-E3-User-Tag");
                }
            }
        }

        /// <summary>
        /// Use the ValidityPeriod property to specify the maximum message delivery validity after which the message is discarded unless received. The property is of type TimeSpan (seconds and milliseconds are ignored). If not set the default validity period is applied. The maximum validity period is 14 days, if you specify a longer validity period 14 days will be used.
        /// </summary>
        public TimeSpan? ValidityPeriod
        {
            get
            {
                if (ContainsKey("X-E3-Validity-Period"))
                {
                    string value = this["X-E3-Validity-Period"];
                    int n = int.Parse(value.Substring(0,
                        value.IndexOfAny(new char[] { 'w', 'd', 'h', 'm' })));

                    if (value.EndsWith("w"))
                    {
                        return new TimeSpan(n * 7, 0, 0, 0);
                    }
                    else if (value.EndsWith("d"))
                    {
                        return new TimeSpan(n, 0, 0, 0);
                    }
                    else if (value.EndsWith("h"))
                    {
                        return new TimeSpan(n, 0, 0);
                    }
                    else if (value.EndsWith("m"))
                    {
                        return new TimeSpan(0, n, 0);
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to parse X-E3-Validity-Period=" + value);
                    }
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value.HasValue)
                {
                    TimeSpan ts = new TimeSpan(
                        value.Value.Days,
                        value.Value.Hours,
                        value.Value.Minutes,
                        0, 0);
                    string s;
                    if (ts.TotalDays >= 7 && ts.TotalDays % 7 == 0)
                        s = ((int)ts.TotalDays / 7) + "w";
                    else if (ts.TotalDays >= 1 && ts.TotalDays % 1 == 0)
                        s = ((int)ts.TotalDays) + "d";
                    else if (ts.TotalHours >= 1 && ts.TotalHours % 1 == 0)
                        s = ((int)ts.Hours) + "h";
                    else
                        s = ((int)ts.TotalMinutes) + "m";
                    this["X-E3-Validity-Period"] = s;
                }
                else
                {
                    Remove("X-E3-Validity-Period");
                }
            }
        }

        /// <summary>
        /// Returns an XML representation of this SendSmsRequest instance.
        /// </summary>
        /// <returns>XML representation of this SendSmsRequest instance.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the SendSmsRequest instance has not yet been intialized.</exception>
        public override string ToString()
        {
            EnsureInitialized();

            XmlSerializer serializer = new XmlSerializer(typeof(SendSmsRequest));
            StringBuilder builder = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(builder,
                   new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true }))
            {
                serializer.Serialize(writer, this);
            }

            return builder.ToString();
        }

        internal void EnsureInitialized()
        {
            if (Messages == null || Messages.Count == 0 ||
                Recipients == null || Recipients.Count == 0)
            {
                throw new InvalidOperationException("SendSmsRequest has not been initialized.");
            }
        }
    }

    /// <summary>
    /// Contains details of an individual submission.
    /// </summary>
    [XmlRoot("sms")]
    public class Sms
    {
        /// <summary>
        /// Unique identifier if the submission was successful; null otherwise.
        /// </summary>
        [XmlAttribute("X-E3-ID")]
        public String Id { get; set; }
        /// <summary>
        /// Recipient of the submission.
        /// </summary>
        [XmlAttribute("X-E3-Recipients")]
        public String Recipient { get; set; }
        /// <summary>
        /// Outcome (status code) of the submission; "00" means successful.
        /// </summary>
        /// <seealso cref="Successful"/>
        [XmlAttribute("X-E3-Submission-Report")]
        public String SubmissionReport { get; set; }
        /// <summary>
        /// Error description if the submission failed; null otherwise.
        /// </summary>
        [XmlAttribute("X-E3-Error-Description")]
        public String ErrorDescription { get; set; }

        /// <summary>
        /// True if the submission was successful; false otherwise.
        /// </summary>
        public bool Successful
        {
            get
            {
                return SubmissionReport == "00";
            }
        }
    }

    /// <summary>
    /// Response object containing a list of one or more submitted messages.
    /// </summary>
    [XmlRoot("sendSmsResponse")]
    public class SendSmsResponse
    {
        /// <summary>
        /// List of submitted messages.
        /// </summary>
        [XmlElement("sms", typeof(Sms))]
        public List<Sms> Messages { get; set; }

        SendSmsResponse() { Messages = new List<Sms>(); }
    }

    /// <summary>
    /// Client used for sending messages.
    /// </summary>
    public class SendSmsClient
    {
        private string _Endpoint;

        /// <summary>
        /// Endpoint (host name) used for sending messages.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown on setting when Endpoint is null or empty.</exception>
        public string Endpoint
        {
            get
            {
                return _Endpoint;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("No Endpoint provided.");
                }

                _Endpoint = value;
            }
        }

        private NetworkCredential _Credentials;

        /// <summary>
        /// Credentials (user name, password) used for sending messages.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">Thrown on setting when Credentials is null.</exception>
        public NetworkCredential Credentials
        {
            get
            {
                return _Credentials;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("No Credentials provided.");
                }

                if (String.IsNullOrEmpty(value.UserName))
                {
                    throw new ArgumentNullException("No UserName provided.");
                }

                if (String.IsNullOrEmpty(value.Password))
                {
                    throw new ArgumentNullException("No Password provided.");
                }

                _Credentials = value;
            }
        }

        private string _Path;

        /// <summary>
        /// Path used for sending messages (normally you should not and don't need to modify this).
        /// </summary>
        public string Path
        {
            get
            {
                return _Path;
            }

            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("No Path provided.");
                }

                if (!value.StartsWith("/"))
                {
                    throw new ArgumentException("The path must start with '/'.");
                }

                _Path = value;
            }
        }

        /// <summary>
        /// Allows enabling (if true) or disabling (if false) secure communication. By default secure communication is enabled and we recommended that you don't change this property.
        /// </summary>
        public bool Secure { get; set; }

        /// <summary>
        /// Creates a new SendSmsClient instance. Messaging endpoint and credentials must be provided separately.
        /// </summary>
        public SendSmsClient()
        {
            Secure = true;
            Path = "/submit_sm";
        }

        /// <summary>
        /// Creates a new SendSmsClient instance with provide messaging endpoint and credentials.
        /// </summary>
        /// <param name="Endpoint">API endpoint (host name) used for sending messages.</param>
        /// <param name="Credentials">API credentials (user name, password) used for sending messages.</param>
        public SendSmsClient(string Endpoint, NetworkCredential Credentials)
        {
            this.Endpoint = Endpoint;
            this.Credentials = Credentials;
            Secure = true;
            Path = "/submit_sm";
        }

        /// <summary>
        /// Performs the message submission.
        /// </summary>
        /// <param name="sendSmsRequest">Request object containing message(s), recipient(s) and other optional properties.</param>
        /// <returns>Response object containing a list of one or more submitted messages.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown if the SendSmsRequest instance has not yet been intialized.</exception>
        /// <exception cref="System.Net.WebException">Thrown if there is a networking or communication problem with the endpoint.</exception>
        public SendSmsResponse SendSms(SendSmsRequest sendSmsRequest)
        {
            sendSmsRequest.EnsureInitialized();        

            Uri baseUri = new Uri((Secure ? Uri.UriSchemeHttps : Uri.UriSchemeHttp) + "://" + Endpoint);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(baseUri, Path));
            webRequest.Method = "POST";
            webRequest.ContentType = "application/xml; charset=UTF-8";
            webRequest.Accept = "application/xml";
            webRequest.Credentials = Credentials;

            // Need this to prevent two requests per submission (without it .NET
            // would perform a request without credentials only to receive a 401
            // status and repeat the request with credentials
            string authInfo = Credentials.UserName + ":" + Credentials.Password;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            webRequest.Headers["Authorization"] = "Basic " + authInfo;

            XmlSerializer serializer1 = new XmlSerializer(typeof(SendSmsRequest));
            serializer1.Serialize(webRequest.GetRequestStream(), sendSmsRequest);

            try
            {
                WebResponse webResponse = webRequest.GetResponse();
                XmlSerializer serializer2 = new XmlSerializer(typeof(SendSmsResponse));
                return serializer2.Deserialize(
                    webResponse.GetResponseStream()) as SendSmsResponse;
            }
            catch (WebException e)
            {
                string message = null;
                try
                {
                    if (e.Status == WebExceptionStatus.ProtocolError)
                    {
                        var response = ((HttpWebResponse)e.Response);
                        using (var stream = response.GetResponseStream())
                        {
                            using (var reader = new StreamReader(stream))
                            {
                                message = reader.ReadToEnd();
                            }
                        }
                    }
                }
                catch (WebException)
                {
                }

                if (message == null)
                    throw e;
                else
                    throw new WebException(e.Message + " [" + message.Trim() + "]", e, e.Status, e.Response);
            }
        }
    }

    /// <summary>
    /// Classifies the delivery report into Delivered, TemporaryError or PermanentError.
    /// </summary>
    public enum State
    {
        /// <summary>
        /// Undefined (null) state
        /// </summary>
        Undefined,
        /// <summary>
        /// Message delivered 
        /// </summary>
        Delivered,
        /// <summary>
        /// Temporary error but could still be delivered
        /// </summary>
        TemporaryError,
        /// <summary>
        /// Permanent error, message was not delivered
        /// </summary>
        PermanentError
    }

    /// <summary>
    /// Allows parsing incoming message report POST requests.
    /// </summary>
    [XmlRoot("callback")]
    public class SmsReport
    {
        private static TimeZoneInfo timeZoneInformation = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

        /// <summary>
        /// Unique report identifier (not that of the original submission).
        /// </summary>
        [XmlAttribute("X-E3-ID")]
        public String Id { get; set; }

        /// <summary>
        /// The original recipient of the submission.
        /// </summary>
        [XmlAttribute("X-E3-Recipients")]
        public String Recipient { get; set; }

        /// <summary>
        /// Delivery report value, 00 to 1F indicating a successful delivery, 20 to 3F indicating a temporary error and 40 to 7F indicating a permanent error.
        /// </summary>
        [XmlAttribute("X-E3-Delivery-Report")]
        public String DeliveryReport { get; set; }

        /// <summary>
        /// Optional UserKey parameter from the message submission. Can be used to associate unique submissions with reports, even if the recipient is the same.
        /// </summary>
        [XmlAttribute("X-E3-User-Key")]
        public String UserKey { get; set; }

        /// <summary>
        /// Date and time the message was received (UTC).
        /// </summary>
        [XmlIgnore]
        public DateTime Timestamp { get; set; }

        /// <see cref="Timestamp"/>
        [XmlAttribute("X-E3-Timestamp")]
        public string TimestampString
        {
            get
            {
                return TimeZoneInfo.ConvertTimeFromUtc(
                    Timestamp, timeZoneInformation).ToString("yyyy-MM-dd HH:mm:ss");
            }
            set
            {
                DateTime dateTime = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", null);
                Timestamp = TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZoneInformation);
            }
        }

        /// <summary>
        /// Mobile operator network name.
        /// </summary>
        [XmlAttribute("X-E3-Network")]
        public String Network { get; set; }

        /// <summary>
        /// The delivery report value classified into State.Delivered, State.TemporaryError or State.PermanentError.
        /// </summary>
        public State State
        {
            get
            {
                if (DeliveryReport == null || DeliveryReport.Length == 0)
                    return State.Undefined;
                if (DeliveryReport.StartsWith("0") || DeliveryReport.StartsWith("1"))
                    return State.Delivered;
                if (DeliveryReport.StartsWith("2") || DeliveryReport.StartsWith("3"))
                    return State.TemporaryError;
                if (DeliveryReport.StartsWith("4") || DeliveryReport.StartsWith("5") || DeliveryReport.StartsWith("6") || DeliveryReport.StartsWith("7"))
                    return State.PermanentError;
                throw new InvalidOperationException(
                    "Unknown delivery report value: " + DeliveryReport
                );
            }
        }

        /// <summary>
        /// True if State is State.Delivered, false otherwise.
        /// </summary>
        public Boolean Successful
        {
            get
            {
                return State == State.Delivered;
            }
        }

        /// <summary>
        /// Returns a string representation of this SmsReport instance.
        /// </summary>
        /// <returns>String representation of this SmsReport instance.</returns>
        public override string ToString()
        {
            return string.Format("Id: {0}, Recipient: {1}, DeliveryReport: {2}, UserKey: {3}, Timestamp: {4}, Network: {5}",
                Id, Recipient, DeliveryReport, UserKey, Timestamp, Network);
        }

        /// <summary>
        /// Parses a report from the given string.
        /// </summary>
        /// <param name="str">Input string.</param>
        /// <returns>The parsed SmsReport instance.</returns>
        public static SmsReport getInstance(string str)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SmsReport));
            return serializer.Deserialize(new StringReader(str)) as SmsReport;
        }

        /// <summary>
        /// Parses a report from the given TextReader.
        /// </summary>
        /// <param name="reader">Input text reader.</param>
        /// <returns>The parsed SmsReport instance.</returns>
        public static SmsReport getInstance(TextReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SmsReport));
            return serializer.Deserialize(reader) as SmsReport;
        }

        /// <summary>
        /// Parses a report from the given Stream.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <returns>The parsed SmsReport instance.</returns>
        public static SmsReport getInstance(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SmsReport));
            return serializer.Deserialize(stream) as SmsReport;
        }
    }

    /// <summary>
    /// Allows parsing incoming message reply POST requests.
    /// </summary>
    [XmlRoot("callback")]
    public class SmsReply
    {
        private static TimeZoneInfo timeZoneInformation = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

        /// <summary>
        /// Unique message identifier (not that of the original submission).
        /// </summary>
        [XmlAttribute("X-E3-ID")]
        public String Id { get; set; }

        /// <summary>
        /// The original recipient of the submission (now the sender as it’s a reply).
        /// </summary>
        [XmlAttribute("X-E3-Originating-Address")]
        public String Sender { get; set; }

        /// <summary>
        /// Optional SessionId parameter from submission. Can be used to associate unique submissions with replies, even if the recipient is the same.
        /// </summary>
        [XmlAttribute("X-E3-Session-ID")]
        public String SessionId { get; set; }

        /// <summary>
        /// The hex-encoded message text.
        /// </summary>
        /// <seealso cref="Message"/>
        [XmlAttribute("X-E3-Hex-Message")]
        public String HexMessage { get; set; }

        /// <summary>
        /// The message text.
        /// </summary>
        public String Message
        {
            get
            {
                if (HexMessage == null)
                    return null;

                byte[] data = new byte[HexMessage.Length / 2];
                for (int i = 0; i < data.Length; i++)
                    data[i] = Convert.ToByte(HexMessage.Substring(i * 2, 2), 16);
                return Encoding.GetEncoding("ISO-8859-15").GetString(data);
            }
        }

        /// <summary>
        /// Date and time the message was received (UTC).
        /// </summary>
        [XmlIgnore]
        public DateTime Timestamp { get; set; }

        /// <see cref="Timestamp"/>
        [XmlAttribute("X-E3-Timestamp")]
        public string TimestampString
        {
            get
            {
                return TimeZoneInfo.ConvertTimeFromUtc(
                    Timestamp, timeZoneInformation).ToString("yyyy-MM-dd HH:mm:ss.000000");
            }
            set
            {
                DateTime dateTime = DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss.000000", null);
                Timestamp = TimeZoneInfo.ConvertTimeToUtc(dateTime, timeZoneInformation);
            }
        }

        /// <summary>
        /// Mobile operator network name.
        /// </summary>
        [XmlAttribute("X-E3-Network")]
        public String Network { get; set; }

        /// <summary>
        /// Returns a string representation of this SmsReply instance.
        /// </summary>
        /// <returns>String representation of this SmsReply instance.</returns>
        public override string ToString()
        {
            return string.Format("Id: {0}, Sender: {1}, SessionId: {2}, HexMessage: {3}, Message: {4}, Timestamp: {5}, Network: {6}",
                Id, Sender, SessionId, HexMessage, Message, Timestamp, Network);
        }

        /// <summary>
        /// Parses a reply from the given string.
        /// </summary>
        /// <param name="str">Input string.</param>
        /// <returns>The parsed SmsReply instance.</returns>
        public static SmsReply getInstance(string str)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SmsReply));
            return serializer.Deserialize(new StringReader(str)) as SmsReply;
        }

        /// <summary>
        /// Parses a reply from the given TextReader.
        /// </summary>
        /// <param name="reader">Input text reader.</param>
        /// <returns>The parsed SmsReply instance.</returns>
        public static SmsReply getInstance(TextReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SmsReply));
            return serializer.Deserialize(reader) as SmsReply;
        }

        /// <summary>
        /// Parses a reply from the given Stream.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <returns>The parsed SmsReply instance.</returns>
        public static SmsReply getInstance(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SmsReply));
            return serializer.Deserialize(stream) as SmsReply;
        }
    }
}