using MMDB.DataService.Data.Dto.Email;
using MMDB.DataService.Data.Impl;
using MMDB.DataService.Data.Settings;
using MMDB.RazorEmail;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace MMDB.DataService.Data.Tests
{
    public class EmailManagerTests : RavenTestBase
    {
        private class TestData
        {
            public Fixture Fixture { get; set; }
            public Mock<IEventReporter> EventReporter { get; set; }
            public Mock<IConnectionSettingsManager> ConnectionSettingsManager { get; set; }
            public Mock<EmailSender> EmailSender { get; set; }
            public EmailJobData EmailJobData { get; set; }
            public EmailConnectionSettings EmailConnectionSettings { get; set; }
            public IEmailManager Sut { get; set; }

            public static TestData Create(IDocumentSession documentSession)
            {
                var fixture = new Fixture();
                var testData = new TestData
                {
                    Fixture = fixture,
                    EventReporter = new Mock<IEventReporter>(),
                    ConnectionSettingsManager = new Mock<IConnectionSettingsManager>(),
                    EmailSender = new Mock<EmailSender>(),
                    EmailJobData = fixture.Create<EmailJobData>(),
                    EmailConnectionSettings = fixture.Create<EmailConnectionSettings>()
                };
                testData.Sut = new EmailManager(documentSession, testData.EventReporter.Object, testData.ConnectionSettingsManager.Object, testData.EmailSender.Object);

                testData.ConnectionSettingsManager.Setup(i=>i.Load<EmailConnectionSettings>(testData.EmailJobData.SettingsSource, testData.EmailJobData.SettingsKey)).Returns(testData.EmailConnectionSettings);

                testData.EmailJobData.FromAddress.EmailAddress = Guid.NewGuid().ToString() + "@example.com";
                foreach(var item in testData.EmailJobData.ToAddressList)
                {
                    item.EmailAddress = Guid.NewGuid().ToString() + "@example.com";
                }

                return testData;
            }
        }

        [Test]
        public void SendsEmail()
        {
            var testData = TestData.Create(this.DocumentSession);

            testData.Sut.ProcessItem(testData.EmailJobData);

            testData.EmailSender.Verify(i=>i.SendEmail(It.IsAny<EmailServerSettings>(), testData.EmailJobData.Subject, testData.EmailJobData.Body, It.IsAny<IEnumerable<MailAddress>>(), It.IsAny<MailAddress>(), It.IsAny<EmailAttachmentData[]>()), Times.Once());
        }

        [Test]
        public void FiltersCarriageReturnsFromSubject()
        {
            var testData = TestData.Create(this.DocumentSession);

            testData.EmailJobData.Subject = "test is\r\nthe subject \r and \n more stuff \r\n";
            string expectedSubject = "test is the subject   and   more stuff  ";
            testData.Sut.ProcessItem(testData.EmailJobData);

            testData.EmailSender.Verify(i => i.SendEmail(It.IsAny<EmailServerSettings>(), expectedSubject, testData.EmailJobData.Body, It.IsAny<IEnumerable<MailAddress>>(), It.IsAny<MailAddress>(), It.IsAny<EmailAttachmentData[]>()), Times.Once());
        }
    }
}
