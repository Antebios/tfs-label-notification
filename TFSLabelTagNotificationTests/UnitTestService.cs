using Microsoft.VisualStudio.TestTools.UnitTesting;
using TFSLabelTagNotification;
using PeterKottas.DotNetCore.WindowsService.Interfaces;

namespace TFSLabelTagNotificationTests
{
    [TestClass]
    public class UnitTestService
    {
        [Ignore]
        [TestMethod]
        public void ServiceRestarts()
        {
            bool _result = RestartService(5000);

            Assert.IsTrue(_result, "Service did not start.");
        }

        [TestMethod]
        public void TestSendEmail()
        {
            TFSLabelCheck service = new TFSLabelCheck();
            bool _result = service.SendEmail("<h5>This is a unit test body.</h5>", "Unit Test Subject");

            Assert.IsTrue(_result, "Email test was not sent.");
        }


        // IMicroServiceController serviceName, 
        public static bool StartService(int timeoutMilliseconds)
        {
            bool isSuccessful = false;
            TFSLabelCheck service = new TFSLabelCheck();
            try
            {
                System.TimeSpan timeout = System.TimeSpan.FromMilliseconds(timeoutMilliseconds);
                service.Start();
                //service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                isSuccessful = true;
            }
            catch
            {
                // ...
                isSuccessful = false;
            }
            return isSuccessful;
        }

        public static bool StopService(int timeoutMilliseconds)
        {
            bool isSuccessful = false;
            TFSLabelCheck service = new TFSLabelCheck();
            try
            {
                System.TimeSpan timeout = System.TimeSpan.FromMilliseconds(timeoutMilliseconds);
                service.Stop();
                //service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                isSuccessful = true;
            }
            catch
            {
                // ...
                isSuccessful = false;
            }
            return isSuccessful;
        }

        public static bool RestartService(int timeoutMilliseconds)
        {
            bool isSuccessful = false;
            TFSLabelCheck service = new TFSLabelCheck();
            try
            {
                int millisec1 = System.Environment.TickCount;
                System.TimeSpan timeout = System.TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();

                // count the rest of the timeout
                int millisec2 = System.Environment.TickCount;
                timeout = System.TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Stop();
                //service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                isSuccessful = true;
            }
            catch
            {
                // ...
                isSuccessful = false;
            }
            return isSuccessful;
        }
    }
}
