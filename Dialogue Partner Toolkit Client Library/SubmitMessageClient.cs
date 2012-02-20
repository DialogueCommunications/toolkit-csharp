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
    [XmlRoot("sendSmsRequest")]
    public class SendSmsRequest : Dictionary<string, string>, IXmlSerializable
    {
        private static TimeZoneInfo timeZoneInformation = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (string recipient in Recipients)
            {
                writer.WriteStartElement("X-E3-Recipients");
                writer.WriteValue(recipient);
                writer.WriteEndElement();
            }

            foreach (string message in Messages)
            {
                writer.WriteStartElement("X-E3-Message");
                writer.WriteValue(message);
                writer.WriteEndElement();
            }

            foreach (string key in Keys)
            {
                writer.WriteStartElement(key);
                writer.WriteValue(this[key]);
                writer.WriteEndElement();
            }
        }

        public List<string> Messages { get; set; }
        public List<string> Recipients { get; set; }

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
                    if ((ts.TotalDays % 1) == 0)
                        s = ((int)ts.TotalDays) + "d";
                    else if ((ts.TotalHours % 1) == 0)
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

        public override string ToString()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SendSmsRequest));
            StringBuilder builder = new StringBuilder();

            using (XmlWriter writer = XmlWriter.Create(builder,
                   new XmlWriterSettings { OmitXmlDeclaration = true, Indent = true }))
            {
                serializer.Serialize(writer, this);
            }

            return builder.ToString();
        }
    }

    [XmlRoot("sms")]
    public class Sms
    {
        [XmlAttribute("X-E3-ID")]
        public String Id { get; set; }
        [XmlAttribute("X-E3-Recipients")]
        public String Recipient { get; set; }
        [XmlAttribute("X-E3-Submission-Report")]
        public String SubmissionReport { get; set; }
        [XmlAttribute("X-E3-Error-Description")]
        public String ErrorDescription { get; set; }

        public bool Successful
        {
            get
            {
                return SubmissionReport == "00";
            }
        }
    }

    [XmlRoot("sendSmsResponse")]
    public class SendSmsResponse
    {
        [XmlElement("sms", typeof(Sms))]
        public List<Sms> Messages { get; set; }

        public SendSmsResponse() { Messages = new List<Sms>(); }
    }

    public class SendSmsClient
    {
        public NetworkCredential Credentials { get; set; }
        public Uri Endpoint { get; set; }
        public string Path { get; set; }
        public bool Secure
        {
            get
            {
                return Endpoint.Scheme == Uri.UriSchemeHttps;
            }

            set
            {
                Endpoint = new Uri((value ? Uri.UriSchemeHttps : Uri.UriSchemeHttp) + "://" +
                    Endpoint.Host + (Endpoint.IsDefaultPort ? "" : ":" + Endpoint.Port));
            }
        }

        public SendSmsClient()
        {
            Endpoint = new Uri("http://sendmsg.dialogue.net");
            Path = "/submit_sm";
        }

        public SendSmsResponse SendSms(SendSmsRequest sendSmsRequest)
        {
            WebRequest webRequest = WebRequest.Create(new Uri(Endpoint, Path));
            webRequest.Method = "POST";
            webRequest.ContentType = "application/xml; charset=UTF-8";
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

    public enum State
    {
        Undefined,
        Delivered,
        TemporaryError,
        PermanentError
    }

    [XmlRoot("callback")]
    public class SmsReport
    {
        private static TimeZoneInfo timeZoneInformation = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

        [XmlAttribute("X-E3-ID")]
        public String Id { get; set; }

        [XmlAttribute("X-E3-Recipients")]
        public String Recipient { get; set; }

        [XmlAttribute("X-E3-Delivery-Report")]
        public String DeliveryReport { get; set; }

        [XmlAttribute("X-E3-User-Key")]
        public String UserKey { get; set; }

        [XmlIgnore]
        public DateTime Timestamp { get; set; }

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

        [XmlAttribute("X-E3-Network")]
        public String Network { get; set; }

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
                if (DeliveryReport.StartsWith("3") || DeliveryReport.StartsWith("4"))
                    return State.PermanentError;
                throw new InvalidOperationException(
                    "Unknown delivery report value: " + DeliveryReport
                );
            }
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, Recipient: {1}, DeliveryReport: {2}, UserKey: {3}, Timestamp: {4}, Network: {5}",
                Id, Recipient, DeliveryReport, UserKey, Timestamp, Network);
        }

        public static SmsReport getInstance(string str)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SmsReport));
            return serializer.Deserialize(new StringReader(str)) as SmsReport;
        }

        public static SmsReport getInstance(TextReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SmsReport));
            return serializer.Deserialize(reader) as SmsReport;
        }

        public static SmsReport getInstance(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SmsReport));
            return serializer.Deserialize(stream) as SmsReport;
        }
    }

    [XmlRoot("callback")]
    public class SmsReply
    {
        private static TimeZoneInfo timeZoneInformation = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

        [XmlAttribute("X-E3-ID")]
        public String Id { get; set; }

        [XmlAttribute("X-E3-Originating-Address")]
        public String Sender { get; set; }

        [XmlAttribute("X-E3-Session-ID")]
        public String SessionId { get; set; }

        [XmlAttribute("X-E3-Hex-Message")]
        public String HexMessage { get; set; }

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

        [XmlIgnore]
        public DateTime Timestamp { get; set; }

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

        [XmlAttribute("X-E3-Network")]
        public String Network { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, Sender: {1}, SessionId: {2}, HexMessage: {3}, Message: {4}, Timestamp: {5}, Network: {6}",
                Id, Sender, SessionId, HexMessage, Message, Timestamp, Network);
        }

        public static SmsReply getInstance(string str)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SmsReply));
            return serializer.Deserialize(new StringReader(str)) as SmsReply;
        }

        public static SmsReply getInstance(TextReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SmsReply));
            return serializer.Deserialize(reader) as SmsReply;
        }

        public static SmsReply getInstance(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SmsReply));
            return serializer.Deserialize(stream) as SmsReply;
        }
    }
}