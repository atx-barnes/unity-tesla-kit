using System;

namespace Tesla.API.Core {

    [Serializable]
    public class DriveState : State {

        public DriveStateResponse Response { get; set; }

        [Serializable]
        public class DriveStateResponse {

            public int gps_as_of { get; set; }
            public int heading { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public double native_latitude { get; set; }
            public int native_location_supported { get; set; }
            public double native_longitude { get; set; }
            public string native_type { get; set; }
            public int power { get; set; }
            public object shift_state { get; set; }
            public object speed { get; set; }
            public double timestamp { get; set; }
        }
    }
}
