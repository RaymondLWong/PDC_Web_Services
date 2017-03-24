using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDC_Web_Services {
    public enum SensorTypeEnum {
        Motion,
        Smoke_Heat_CO,
        Vibration,
        Glass_break,
        Entry
    }
    public class Sensor {
        SensorTypeEnum type;
        string value;
    }
}