using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDC_Web_Services {
    public class Event {
        public Sensor[] sensors;
    }
    public class EventNotification {
        public int notificationID;
        public SystemState state;
        public DateTime timestamp;
        public Event[] events;
    }
}