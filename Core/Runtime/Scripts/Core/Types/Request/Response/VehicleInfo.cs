using System.Collections.Generic;
using UnityEngine;

namespace Tesla.API.Core
{
    /// <summary>
    /// Vehicle response object, more info at https://tesla-api.timdorr.com/api-basics/vehicles
    /// </summary>
    [SerializeField]
    public class VehicleInfo : State
    {
        public VehicleInfoResponse Response { get; set; }

        [SerializeField]
        public class VehicleInfoResponse
        {
            public long id { get; set; }
            public int vehicle_id { get; set; }
            public string vin { get; set; }
            public string display_name { get; set; }
            public string option_codes { get; set; }
            public object color { get; set; }
            public string[] tokens { get; set; }
            public string state { get; set; }
            public bool in_service { get; set; }
            public string id_s { get; set; }
            public bool calendar_enabled { get; set; }
            public int api_version { get; set; }
            public object backseat_token { get; set; }
            public object backseat_token_updated_at { get; set; }
        }
    }
}