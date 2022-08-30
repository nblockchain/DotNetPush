using System;
using System.IO;
using System.Net.Sockets;
using System.Net.Security;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using PushSharp.Core;
using dotAPNS;

namespace PushSharp.Apple
{
    public class ApnsConnection
    {
        public ApnsConnection (ApnsConfiguration configuration)
        {
            Configuration = configuration;

            certificate = Configuration.Certificate;

            client =
                ApnsClient
                .CreateUsingCert(certificate);

            if (configuration.UseBackupPort)
                client.UseBackupPort();
        }

        public ApnsConfiguration Configuration { get; private set; }

        private readonly X509Certificate2 certificate;
        private readonly ApnsClient client;

        public async Task SendAsync (ApnsNotification notification)
        {
            ApnsResponse response;

            if (Configuration.ServerEnvironment == ApnsConfiguration.ApnsServerEnvironment.Production)
                response = await client.SendAsync(notification.ToApplePush());
            else
            {
                var applePushToSend =
                    notification
                        .ToApplePush()
                        .SendToDevelopmentServer();

                response = await client.SendAsync(applePushToSend);
            }

            if (response.IsSuccessful)
                return;
            else
                throw new NotificationException($"Notification send failed, Reason = {response.ReasonString}", notification);
        }
    }
}
