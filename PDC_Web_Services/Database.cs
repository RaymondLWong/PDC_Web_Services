﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDC_Web_Services {
    public class Database {

        private static MySqlConnection con = null;
        private static int lastInsertID = -1;

        public static MySqlConnection getDBConection() {
            if (con == null) {
                con = new MySqlConnection(Properties.Settings.Default.pdcConnectionString);
            }

            return con;
        }

        public static bool createNewNotification(int roomID) {
            bool success = false;

            MySqlConnection con = getDBConection();

            MySqlCommand cmd = new MySqlCommand(@"
INSERT INTO notifications (
	timestamp,
	roomID
) VALUES (
	@ts,
	@room
)", con);

            MySqlParameter paramTimestamp = new MySqlParameter("@ts", MySqlDbType.DateTime);
            MySqlParameter paramRoomID = new MySqlParameter("@room", MySqlDbType.Int32);

            paramTimestamp.Value = DateTime.Now;
            paramRoomID.Value = roomID;

            cmd.Parameters.Add(paramTimestamp);
            cmd.Parameters.Add(paramRoomID);

            try {
                con.Open();
                cmd.ExecuteNonQuery();

                // http://stackoverflow.com/questions/15373851/c-sharp-get-insert-id-with-auto-increment
                lastInsertID = (int)cmd.LastInsertedId;
                success = true;
            } catch (MySqlException MySqlE) {
                throw MySqlE;
            } finally {
                con.Close();
            }

            return success;
        }

        public static bool createReading(int sensorID, string value, int notificationID) {
            bool success = false;

            MySqlConnection con = getDBConection();

            MySqlCommand cmd = new MySqlCommand(@"
INSERT INTO events (
    notificationID,
    sensorID, 
    sensorValue
) VALUES (
    @groupID,
    @sensorID,
    @sensorValue
)", con);

            MySqlParameter paramGroupID = new MySqlParameter("@groupID", MySqlDbType.Int32);
            MySqlParameter paramSensorID = new MySqlParameter("@sensorID", MySqlDbType.Int32);
            MySqlParameter paramSensorValue = new MySqlParameter("@sensorValue", MySqlDbType.VarChar);

            paramGroupID.Value = notificationID;
            paramSensorID.Value = sensorID;
            paramSensorValue.Value = value;

            cmd.Parameters.Add(paramGroupID);
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

        public static bool submitReadingAndCreateGroup(int roomID, int sensorID, string value, int notificationID) {
            // establish whether this is creating a new notification or adding to an existing one
            if (notificationID == -1) {
                bool groupCreated = createNewNotification(roomID);
                int newNotificationID = lastInsertID;
                Console.WriteLine($"new notification: {newNotificationID}");

                if (groupCreated) {
                    return createReading(sensorID, value, newNotificationID);
                } else {
                    Console.WriteLine("Error: Notification group could not be created.");
                    return false;
                }
            } else {
                return createReading(sensorID, value, notificationID);
            }
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

            paramState.Value = (enable) ? 1 : 2;
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
    }
}