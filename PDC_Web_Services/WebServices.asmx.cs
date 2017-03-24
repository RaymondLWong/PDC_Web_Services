using System;
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
        public bool submitReading(int sensorID, string value) {
            return Database.createReading(sensorID, value);
        }

        [WebMethod]
        public string checkHome(int homeID) {
            return Database.getHomeState(homeID);
        }

        [WebMethod]
        public bool resetSystem(int homeID) {
            Database.toggleAlarmState(homeID, false);
            return Database.toggleAlarmState(homeID, true);
        }

        [WebMethod]
        public bool toogleSystem(int homeID, bool enable) {
            return Database.toggleAlarmState(homeID, enable);
        }

        [WebMethod]
        public XmlDocument fetchEventsWithinTimeframe(int homeID, DateTime start, DateTime end) {
            return Database.getEventsWithinTimeframe(homeID, start, end);
        }

        [WebMethod]
        public XmlDocument fetchEventsSinceSystemArmed(int homeID) {
            return Database.getEventsSinceSystemArmed(homeID);
        }

        [WebMethod]
        public DateTime getSystemStartupTime(int homeID) {
            return Database.getRecentSystemStartupTime(homeID);
        }
    }
}
