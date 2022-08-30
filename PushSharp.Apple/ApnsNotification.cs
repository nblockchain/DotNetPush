using System;
using PushSharp.Core;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
using dotAPNS;

namespace PushSharp.Apple
{
    public class ApnsNotification : INotification
    {
        public object Tag { get; set; }

        public int Identifier { get; private set; }

        public string DeviceToken { get; set; }

        public string Title { get; set; }

        public string Subtitle { get; set; }

        public string Body { get; set; } = string.Empty;

        public int? Badge { get; set; }

        public string Sound { get; set; }

        /// <summary>
        /// The expiration date after which Apple will no longer store and forward this push notification.
        /// If no value is provided, an assumed value of one year from now is used.  If you do not wish
        /// for Apple to store and forward, set this value to Notification.DoNotStore.
        /// </summary>
        public DateTimeOffset? Expiration { get; set; }

        public bool LowPriority { get; set; }

        private const int DEVICE_TOKEN_STRING_MIN_SIZE = 64;

        public static readonly DateTimeOffset DoNotStore = DateTimeOffset.MinValue;

        public ApnsNotification () : this (null, null, null, null)
        {
        }

        public ApnsNotification(string deviceToken, string title, string subtitle, string body)
        {
            if (!string.IsNullOrEmpty(deviceToken) && deviceToken.Length < DEVICE_TOKEN_STRING_MIN_SIZE)
                throw new NotificationException("Invalid DeviceToken Length", this);

            DeviceToken = deviceToken;
            Title = title;
            Subtitle = subtitle;
            Body = body;
        }

        public bool IsDeviceRegistrationIdValid ()
        {
            var r = new System.Text.RegularExpressions.Regex (@"^[0-9A-F]+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            return r.Match (this.DeviceToken).Success;
        }

        internal ApplePush ToApplePush()
        {
            if (Body != null)
                throw new Exception($"{nameof(Body)} can't be null or empty");

            if (string.IsNullOrWhiteSpace(Sound))
                throw new Exception($"{nameof(Sound)} can't be null or empty");
            if (string.IsNullOrWhiteSpace(DeviceToken) || DeviceToken.Length < DEVICE_TOKEN_STRING_MIN_SIZE)
                throw new Exception($"{nameof(DeviceToken)} can't be null or empty");

            var applePush =
                new ApplePush(ApplePushType.Alert)
                    .AddAlert(Title, Subtitle, Body)
                    .AddToken(DeviceToken)
                    .SetPriority(LowPriority ? 5 : 10);

            if (!string.IsNullOrWhiteSpace(Sound))
                applePush.AddSound(Sound);

            if (Badge.HasValue)
                applePush.AddBadge(Badge.Value);

            if (Expiration.HasValue)
                applePush.AddExpiration(Expiration.Value);

            return applePush;
        }
    }
}

