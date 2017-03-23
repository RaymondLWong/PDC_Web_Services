using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;

namespace PDC_Web_Services {
    /// <summary>
    /// Summary description for WebServices
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebServices : System.Web.Services.WebService {

        [WebMethod]
        public bool createNewNotification(int roomID) {
            return Database.createNewNotification(roomID);
        }

        [WebMethod]
        public bool createReading(int sensorID, string value, int notificationID) {
            return Database.createReading(sensorID, value, notificationID);
        }

        [WebMethod]
        public bool submitReadingAndGroup(int homeID, int roomID, int sensorID, string value, int notificationID = -1) {
            bool success = false;

            return success;
        }

        [WebMethod]
        public XmlDocument checkHome(int homeID) {
            return new XmlDocument();
        }

        [WebMethod]
        public bool resetSystem(int homeID) {
            bool success = false;

            return success;
        }

        [WebMethod]
        public bool toogleSystem(int homeID, bool enable) {
            bool success = false;

            Database.toggleAlarmState(homeID, enable);

            return success;
        }
    }
}
