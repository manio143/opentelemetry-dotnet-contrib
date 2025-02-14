// <copyright file="ServerCertificateValidationProviderTests.cs" company="OpenTelemetry Authors">
// Copyright The OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

#if !NETFRAMEWORK

using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using OpenTelemetry.ResourceDetectors.AWS.Http;
using Xunit;

namespace OpenTelemetry.ResourceDetectors.AWS.Tests.Http;

public class ServerCertificateValidationProviderTests
{
    private const string INVALIDCRTNAME = "invalidcert";

    [Fact]
    public void TestValidCertificate()
    {
        // This test fails on Linux in netcoreapp3.1, but passes in net6.0 and net7.0.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            using (CertificateUploader certificateUploader = new CertificateUploader())
            {
                certificateUploader.Create();

                // Loads the certificate to the trusted collection from the file
                ServerCertificateValidationProvider serverCertificateValidationProvider =
                    ServerCertificateValidationProvider.FromCertificateFile(certificateUploader.FilePath);

                // Validates if the certificate loaded into the trusted collection.
                Assert.True(serverCertificateValidationProvider.IsCertificateLoaded);

                var certificate = new X509Certificate2(certificateUploader.FilePath);
                X509Chain chain = new X509Chain();
                chain.Build(certificate);

                // validates if certificate is valid
                Assert.NotNull(serverCertificateValidationProvider);
                Assert.NotNull(serverCertificateValidationProvider.ValidationCallback);
                Assert.True(serverCertificateValidationProvider.ValidationCallback(null, certificate, chain, System.Net.Security.SslPolicyErrors.None));
            }
        }
    }

    [Fact]
    public void TestInValidCertificate()
    {
        // Loads the certificate to the trusted collection from the file
        ServerCertificateValidationProvider serverCertificateValidationProvider =
            ServerCertificateValidationProvider.FromCertificateFile(INVALIDCRTNAME);

        // Validates if the certificate file loaded.
        Assert.False(serverCertificateValidationProvider.IsCertificateLoaded);
    }
}

#endif
