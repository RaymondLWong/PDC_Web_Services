using System.Web.Services;

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
        public bool submitReadingAndCreateGroup(int roomID, int sensorID, string value, int notificationID = -1) {
            return Database.submitReadingAndCreateGroup(roomID, sensorID, value, notificationID);
        }

        [WebMethod]
        public string checkHome(int homeID) {
            return Database.getHomeState(homeID);
        }

        [WebMethod]
        public bool resetSystem(int homeID) {
            // assuming resetting the system rearms it (true = enable, false = disable)
            return Database.toggleAlarmState(homeID, true);
        }

        [WebMethod]
        public bool toogleSystem(int homeID, bool enable) {
            return Database.toggleAlarmState(homeID, enable);
        }
    }
}
