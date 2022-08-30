﻿using System;
using NUnit.Framework;
using PushSharp.Apple;
using Newtonsoft.Json.Linq;

namespace PushSharp.Tests
{
    [TestFixture]
    [Category ("Disabled")]
    public class ApnsRealTest
    {
        [Test]
        public void APNS_Send_Single ()
        {
            var succeeded = 0;
            var failed = 0;
            var attempted = 0;

            var config = new ApnsConfiguration (ApnsConfiguration.ApnsServerEnvironment.Sandbox, Settings.Instance.ApnsCertificateFile, Settings.Instance.ApnsCertificatePassword);
            var broker = new ApnsServiceBroker (config);
            broker.OnNotificationFailed += (notification, exception) => {
                failed++;
            };
            broker.OnNotificationSucceeded += (notification) => {
                succeeded++;
            };
            broker.Start ();

            foreach (var dt in Settings.Instance.ApnsDeviceTokens) {
                attempted++;
                broker.QueueNotification (new ApnsNotification {
                    DeviceToken = dt,
                    Body = "Hello PushSharp!"
                });
            }

            broker.Stop ();

            Assert.AreEqual (attempted, succeeded);
            Assert.AreEqual (0, failed);
        }
    }
}

