using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;

namespace PDC_Web_Services {
    public class Database {

        private const string SENSOR_LOG_TABLE_NAME = "sensorlogs";

        private static MySqlConnection con = null;

        public static MySqlConnection getDBConection() {
            if (con == null) {
                con = new MySqlConnection(Properties.Settings.Default.pdcConnectionString);
            }

            return con;
        }

        public static bool createSystemStateChangeEvent(int homeID, int state) {
            bool success = false;

            MySqlConnection con = getDBConection();

            MySqlCommand cmd = new MySqlCommand(@"
INSERT INTO homelogs (
	homeID,
	timestamp,
    state
) VALUES (
	@id,
	@ts,
    @state
)", con);

            MySqlParameter paramHomeID = new MySqlParameter("@id", MySqlDbType.Int32);
            MySqlParameter paramTimestamp = new MySqlParameter("@ts", MySqlDbType.DateTime);
            MySqlParameter paramState = new MySqlParameter("@state", MySqlDbType.Enum);

            paramHomeID.Value = homeID;
            paramTimestamp.Value = DateTime.Now;
            paramState.Value = state;

            cmd.Parameters.Add(paramHomeID);
            cmd.Parameters.Add(paramTimestamp);
            cmd.Parameters.Add(paramState);

            try {
                con.Open();
                cmd.ExecuteNonQuery();

                success = true;
            } catch (MySqlException MySqlE) {
                throw MySqlE;
            } finally {
                con.Close();
            }

            return success;
        }

        public static bool createReading(int sensorID, string value) {
            bool success = false;

            MySqlConnection con = getDBConection();

            MySqlCommand cmd = new MySqlCommand(@"
INSERT INTO sensorlogs (
    timestamp,
    sensorID, 
    sensorValue
) VALUES (
    @ts,
    @sensorID,
    @sensorValue
)", con);

            MySqlParameter paramTimestamp = new MySqlParameter("@ts", MySqlDbType.DateTime);
            MySqlParameter paramSensorID = new MySqlParameter("@sensorID", MySqlDbType.Int32);
            MySqlParameter paramSensorValue = new MySqlParameter("@sensorValue", MySqlDbType.VarChar);

            paramTimestamp.Value = DateTime.Now;
            paramSensorID.Value = sensorID;
            paramSensorValue.Value = value;

            cmd.Parameters.Add(paramTimestamp);
            cmd.Parameters.Add(paramSensorID);
            cmd.Parameters.Add(paramSensorValue);

            try {
                con.Open();
                cmd.ExecuteNonQuery();

                success = true;
            } catch (MySqlException MySqlE) {
                throw MySqlE;
            } finally {
                con.Close();
            }

            return success;
        }

        public static bool toggleAlarmState(int homeID, bool enable) {
            bool success = false;

            MySqlConnection con = getDBConection();

            MySqlCommand cmd = new MySqlCommand(@"
UPDATE home 
SET alarmState = @state
WHERE homeID = @id
", con);

            MySqlParameter paramState = new MySqlParameter("@state", MySqlDbType.Int32);
            MySqlParameter paramID = new MySqlParameter("@id", MySqlDbType.Enum);

            int state = (enable) ? 1 : 2;

            paramState.Value = state;
            paramID.Value = homeID;

            cmd.Parameters.Add(paramState);
            cmd.Parameters.Add(paramID);

            try {
                con.Open();
                cmd.ExecuteNonQuery();

                success = true;
            } catch (MySqlException MySqlE) {
                throw MySqlE;
            } finally {
                con.Close();
            }

            if (success) {
                // log the change in state
                success = createSystemStateChangeEvent(homeID, state);
            }

            return success;
        }

        public static string getHomeState(int homeID) {
            MySqlConnection con = getDBConection();

            MySqlCommand cmd = new MySqlCommand(@"
SELECT alarmState
FROM home
WHERE homeID = @home
", con);

            MySqlParameter paramID = new MySqlParameter("@home", MySqlDbType.Int32);

            paramID.Value = homeID;
            cmd.Parameters.Add(paramID);

            string state = "unknown";

            try {
                con.Open();
                cmd.ExecuteNonQuery();

                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read()) {
                    state = reader.GetString(0);
                } else {
                    Console.WriteLine(String.Format("System state for home '{0}' not found.", homeID));
                }

                reader.Close();
            } catch (MySqlException MySqlE) {
                throw MySqlE;
            } finally {
                con.Close();
            }

            return state;
        }

        public static XmlDocument getEventsWithinTimeframe(int homeID, DateTime start, DateTime end) {
            MySqlConnection con = getDBConection();

            MySqlCommand cmd = new MySqlCommand(@"
SELECT * FROM sensorlogs WHERE sensorID = 
    ANY ( SELECT sensorID FROM sensors WHERE roomID = 
            ANY (SELECT roomID FROM rooms WHERE homeID = @homeID)
        )
    AND timestamp BETWEEN @start AND @end
", con);

            MySqlParameter paramHomeID = new MySqlParameter("@homeID", MySqlDbType.Int32);
            MySqlParameter paramStartTime = new MySqlParameter("@start", MySqlDbType.DateTime);
            MySqlParameter paramEndTime = new MySqlParameter("@end", MySqlDbType.DateTime);

            paramHomeID.Value = homeID;
            paramStartTime.Value = start;
            paramEndTime.Value = end;

            cmd.Parameters.Add(paramHomeID);
            cmd.Parameters.Add(paramStartTime);
            cmd.Parameters.Add(paramEndTime);

            XmlDocument xmlDom = new XmlDocument();
            xmlDom.AppendChild(xmlDom.CreateElement(SENSOR_LOG_TABLE_NAME));

            try {
                con.Open();
                cmd.ExecuteNonQuery();

                MySqlDataAdapter dataAdptr = new MySqlDataAdapter();
                dataAdptr.SelectCommand = cmd;
                DataSet ds = new DataSet(SENSOR_LOG_TABLE_NAME);
                dataAdptr.Fill(ds, SENSOR_LOG_TABLE_NAME);

                xmlDom.LoadXml(ds.GetXml());

            } catch (MySqlException MySqlE) {
                throw MySqlE;
            } finally {
                con.Close();
            }

            
            return xmlDom;
        }
    }
}