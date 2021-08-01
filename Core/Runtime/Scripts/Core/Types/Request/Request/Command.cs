using System;

namespace Tesla.API.Core {

    /// <summary>
    /// Command response object, more info at https://tesla-api.timdorr.com/vehicle/state/drivestate#get-api-1-vehicles-id-data_request-drive_state
    /// </summary>
    [Serializable]
    public class Command {

        public CommandResponse Response { get; set; }

        [Serializable]
        public class CommandResponse {

            public string reason { get; set; }
            public bool result { get; set; }
        }
    }
}
