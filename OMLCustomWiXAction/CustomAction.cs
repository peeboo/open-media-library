using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using Microsoft.Deployment.WindowsInstaller;

namespace OMLCustomWiXAction {
    public class CustomActions {
        [CustomAction]
        public static ActionResult StartOMLEngineService(Session session) {
            try {
                ServiceController omlengineController = new ServiceController(@"OMLEngineService");
                TimeSpan timeout = TimeSpan.FromSeconds(20);
                omlengineController.Start();
                omlengineController.WaitForStatus(ServiceControllerStatus.Running, timeout);
                omlengineController.Close();
            } catch (Exception e) {
                session.Log("Error starting OMLEngineService: {0}", e.Message);
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult StartOMLFWService(Session session) {
            try {
                ServiceController omlfsserviceController = new ServiceController(@"OMLFWService");
                TimeSpan timeout = TimeSpan.FromSeconds(10);
                omlfsserviceController.Start();
                omlfsserviceController.WaitForStatus(ServiceControllerStatus.Running, timeout);
                omlfsserviceController.Close();
            } catch (Exception e) {
                session.Log("An error occured starting the OMLFW Service: {0}", e.Message);
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult ValidateDatabase(Session session) {
            return ActionResult.Success;
        }
    }
}
