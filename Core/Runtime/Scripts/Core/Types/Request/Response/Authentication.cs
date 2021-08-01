using System;

namespace Tesla.API.Core {

    /// <summary>
    /// Authentication response object. More info at https://tesla-api.timdorr.com/api-basics/authentication#post-oauth-token-grant_type-password
    /// </summary>
    [Serializable]
    public class Authentication {

        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public int created_at { get; set; }
    }
}