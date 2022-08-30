using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace PushSharp.Apple
{
    public class ApnsConfiguration
    {
        public ApnsConfiguration (ApnsServerEnvironment serverEnvironment, string certificateFile, string certificateFilePwd, bool validateIsApnsCertificate)
            : this (serverEnvironment, System.IO.File.ReadAllBytes (certificateFile), certificateFilePwd, validateIsApnsCertificate)
        {
        }

        public ApnsConfiguration (ApnsServerEnvironment serverEnvironment, string certificateFile, string certificateFilePwd)
            : this (serverEnvironment, System.IO.File.ReadAllBytes (certificateFile), certificateFilePwd)
        {
        }

        public ApnsConfiguration (ApnsServerEnvironment serverEnvironment, byte[] certificateData, string certificateFilePwd, bool validateIsApnsCertificate)
            : this (serverEnvironment, new X509Certificate2 (certificateData, certificateFilePwd,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable), validateIsApnsCertificate)
        {
        }

        public ApnsConfiguration (ApnsServerEnvironment serverEnvironment, byte[] certificateData, string certificateFilePwd)
            : this (serverEnvironment, new X509Certificate2 (certificateData, certificateFilePwd,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable))
        {
        }

        public ApnsConfiguration (ApnsServerEnvironment serverEnvironment, X509Certificate2 certificate)
        {
            Initialize (serverEnvironment, certificate, true);
        }

        public ApnsConfiguration (ApnsServerEnvironment serverEnvironment, X509Certificate2 certificate, bool validateIsApnsCertificate)
        {
            Initialize (serverEnvironment, certificate, validateIsApnsCertificate);
        }

        void Initialize (ApnsServerEnvironment serverEnvironment, X509Certificate2 certificate, bool validateIsApnsCertificate)
        {
            ServerEnvironment = serverEnvironment;

            Certificate = certificate;

            if (validateIsApnsCertificate)
                CheckIsApnsCertificate ();
        }


        void CheckIsApnsCertificate ()
        {
            if (Certificate != null) {
                var issuerName = Certificate.IssuerName.Name;
                var commonName = Certificate.SubjectName.Name;

                if (!issuerName.Contains ("Apple"))
                    throw new ArgumentOutOfRangeException ("Your Certificate does not appear to be issued by Apple!  Please check to ensure you have the correct certificate!");

                if (!Regex.IsMatch (commonName, "Apple.*?Push Services")
                    && !commonName.Contains ("Website Push ID:"))
                    throw new ArgumentOutOfRangeException ("Your Certificate is not a valid certificate for connecting to Apple's APNS servers");

                if (commonName.Contains ("Development") && ServerEnvironment != ApnsServerEnvironment.Sandbox)
                    throw new ArgumentOutOfRangeException ("You are using a certificate created for connecting only to the Sandbox APNS server but have selected a different server environment to connect to.");

                if (commonName.Contains ("Production") && ServerEnvironment != ApnsServerEnvironment.Production)
                    throw new ArgumentOutOfRangeException ("You are using a certificate created for connecting only to the Production APNS server but have selected a different server environment to connect to.");
            } else {
                throw new ArgumentOutOfRangeException ("You must provide a Certificate to connect to APNS with!");
            }
        }

        public bool UseBackupPort { get; set; }

        public X509Certificate2 Certificate { get; private set; }

        /// <summary>
        /// Gets the configured APNS server environment 
        /// </summary>
        /// <value>The server environment.</value>
        public ApnsServerEnvironment ServerEnvironment { get; private set; }

        public enum ApnsServerEnvironment {
            Sandbox,
            Production
        }
    }
}