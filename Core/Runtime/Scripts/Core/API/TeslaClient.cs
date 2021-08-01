using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.Events;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Text;
using System;

namespace Tesla.API.Core {

    /// <summary>
    /// For more information about the Unoffical Tesla API checkout https://tesla-api.timdorr.com/
    /// Tesla Client singleton handles user authentication, vehicles, and more.
    /// </summary>
    public sealed class TeslaClient {

        #region Properties

        /// <summary>
        /// Base auth code request endpoint for initial login page retrieval.
        /// </summary>
        private const string authCodeRequestBaseEndpoint = "https://auth.tesla.com/oauth2/v3/authorize";

        /// <summary>
        /// Base endpoint for bearer token exchange for owner api access token.
        /// </summary>
        private const string accessTokenExchangeEndpoint = "https://owner-api.teslamotors.com/oauth/token";

        /// <summary>
        /// Redirect for bearer token exchange request.
        /// </summary>
        private const string redirectUri = "https://auth.tesla.com/void/callback";

        /// <summary>
        /// Base endpoint for bearer token retrieval for access token exchange request.
        /// </summary>
        private const string bearerTokenRequestEndpoint = "https://auth.tesla.com/oauth2/v3/token";

        /// <summary>
        /// The Client ID is a public identifier for apps, in this case this is Tesla's client ID. Find both Client Id and Serect for Tesla API here https://pastebin.com/pS7Z6yyP
        /// </summary>
        private const string clientID = "81527cff06843c8634fdc09e8ac0abefb46ac849f38fe1e431c2ef2106796384";

        /// <summary>
        /// The Client secret is a secret known only to this application and the Tesla authorization server.
        /// </summary>
        private const string clientSecret = "c7257eb71a564034f9419ee651c7d0e5f7aa6bfbd18bafb5c5c033b093bb2fa3";

        /// <summary>
        /// Hidden input form elements to be parsed and used as params for auth code request content.
        /// </summary>
        private readonly string[] hiddenInputFormElements = { "_csrf", "_phase", "_process", "transaction_id", "cancel" };

        /// <summary>
        /// Base URI for all vehicle API requests.
        /// </summary>
        private const string baseVehicleRequestsUri = "https://owner-api.teslamotors.com";

        /// <summary>
        /// User object once authenticated with server. Contains the list of user vehicles for requests.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Single Tesla client static instance.
        /// </summary>
        private static TeslaClient instance;

        public static TeslaClient Instance { get => instance; private set => instance = value; }

        /// <summary>
        /// Path to config file used to save auth information including user access token.
        /// </summary>
        private string configPath;

        public string ConfigPath { private get => configPath; set => configPath = value; }

        /// <summary>
        /// If true, a json file will be serialized at Application.persistentDataPath with the users access token for future requests.
        /// </summary>
        private bool rememberLogin = false;

        public bool RememberLogin { private get => rememberLogin; set => rememberLogin = value; }

        /// <summary>
        /// Request timeout for all requests to Tesla server,
        /// set at 30 seconds by default for the wake command.
        /// </summary>
        private int requestTimeout = 30;

        private int RequestTimeout { get => requestTimeout; set => requestTimeout = value; }

        /// <summary>
        /// Request timeout for all requests to Tesla server,
        /// set at 30 seconds by default for the wake command.
        /// </summary>
        private int vehicleWakeTimeout = 60;

        public int VehicleWakeTimeout { get => vehicleWakeTimeout; set => vehicleWakeTimeout = value; }

        /// <summary>
        /// Authentication information for the current instance.
        /// </summary>
        private Authentication authentication;

        /// <summary>
        /// Http client used to send all endpoint requests.
        /// </summary>
        private HttpClient client;

        /// <summary>
        /// Configure to wake up the vehicles once client has been created and vehicles retrieved.
        /// </summary>
        private bool wakeVehiclesOnVehiclesRetrieved = true;

        public bool WakeVehiclesOnVehiclesRetrieved { private get => wakeVehiclesOnVehiclesRetrieved; set => wakeVehiclesOnVehiclesRetrieved = value; }

        /// <summary>
        /// Authentication property creates a new user component for Unity runtime once authentication has been received.
        /// </summary>
        public Authentication Authentication {

            get {

                if(authentication == null) {

                    throw new TeslaException("You are not authenticated.");

                } else {

                    return authentication;
                }
            }

            set {

                if(value != null) {

                    authentication = value;

                    OnAuthenticated?.Invoke();

                    if (DebugLevel == LogLevel.Client) Debug.Log("User Authenticated");

                } else {

                    if (DebugLevel == LogLevel.Client) Debug.Log("User Not Authenticated");
                }
            }
        }

        /// <summary>
        /// Event invoked when the client checks for saved user authentication. Returns true if user authentication is saved.
        /// </summary>
        public UnityEvent OnRememberLogin;

        /// <summary>
        /// Event invoked when the client has been authenticated either from local cache or new auth request.
        /// </summary>
        public UnityEvent OnAuthenticated;

        /// <summary>
        /// Event invoked when a new user has been created.
        /// </summary>
        public UserInitialized OnUserInitialized;

        /// <summary>
        /// Event invoked when vehicles have retrieved.
        /// </summary>
        public UserVehiclesRetrieved OnUserVehiclesRetrieved;

        /// <summary>
        /// Event invoked when vehicles have awaken.
        /// </summary>
        public UserVehiclesAwaken OnUserVehiclesAwaken;

        /// <summary>
        /// Event invoked when authentication has reset.
        /// </summary>
        public UnityEvent OnClientCacheReset;

        /// <summary>
        /// Enum for the current log level of the client instance. Set to None by default.
        /// </summary>
        public LogLevel DebugLevel = LogLevel.Client;

        public enum LogLevel { None, Http, Client, Vehicles }

        #endregion

        #region Main Methods

        /// <summary>
        /// Default Client constructor, once instantiated client starts initialization.
        /// </summary>
        public TeslaClient() {

            Initialize();
        }

        /// <summary>
        /// Client constructor overload. Configure log level option and ability to wake up the vehicles once retrieved.
        /// </summary>
        public TeslaClient(LogLevel level, bool wakeVehiclesOnVehiclesRetrieved) {

            DebugLevel = level;

            WakeVehiclesOnVehiclesRetrieved = wakeVehiclesOnVehiclesRetrieved;

            Initialize();
        }

        /// <summary>
        /// First method that gets called to initilize the client to retrieve the auth info for the user.
        /// </summary>
        private async void Initialize() {

            configPath = Path.Combine(Application.persistentDataPath, "config.json");

            Instance = this;

            if (File.Exists(configPath)) {

                string contents = File.ReadAllText(configPath);

                Authentication = JsonConvert.DeserializeObject<Authentication>(contents);

                if (DebugLevel == LogLevel.Client) Debug.Log("Found Login Credentials From Previous Session");

                OnRememberLogin?.Invoke();

                await InitializeUserAsync();

            } else {

                if (DebugLevel == LogLevel.Client) Debug.Log("Did Not Find Login Credentials From Previous Session");
            }
        }

        /// <summary>
        /// Handles Tesla account login. Sends request with login credentials to oauth endpoints. More info at https://tesla-api.timdorr.com/api-basics/authentication#post-oauth-token-grant_type-password
        /// Tesla's SSO service has a WAF (web application firewall) that may temporarily block you if you make repeated, execessive requests.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        public async Task AuthenticateAsync(string email, string password) {

            if(authentication != null) {

                Debug.LogWarning("You are already authenticated during this instance");

                return;
            }

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password)) {

                Authentication = await OAuthRequestsAsync(email, password);

                if (rememberLogin && !File.Exists(configPath)) {

                    string json = JsonConvert.SerializeObject(authentication);

                    File.WriteAllText(configPath, json);
                }

                await InitializeUserAsync();

            } else {

                Debug.LogWarning("Username and or password cannot be empty, please try again");
            }
        }

        /// <summary>
        /// Initializes the user once client has been authenticated.
        /// </summary>
        private async Task InitializeUserAsync() {

            User = new User {

                OnVehiclesRetrieved = OnUserVehiclesRetrieved,

                OnUserVehiclesAwaken = OnUserVehiclesAwaken
            };

            OnUserInitialized?.Invoke(User);

            if (DebugLevel == LogLevel.Client) Debug.Log("User Initialized");

            await User.RetrieveVehiclesAsync(wakeVehiclesOnVehiclesRetrieved);
        }

        /// <summary>
        /// Generic request method for getting state or sending a command for a vehicle.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="onRequestComplete"></param>
        internal async Task<T> ClientRequestAsync<T>(string endpoint) {

            if (typeof(T).IsSubclassOf(typeof(State))) {

                return await GetRequestAsync<T>(endpoint);

            } else if (typeof(T).IsSubclassOf(typeof(Command)) || typeof(T) == typeof(Command)) {

                return await PostRequestAsync<T>(endpoint, new WWWForm());

            } else { throw new TeslaException($"The type {typeof(T).Name} is not supported"); }
        }

        /// <summary>
        /// GET request for Tesla API.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        private async Task<T> GetRequestAsync<T>(string endpoint) {

            using UnityWebRequest www = UnityWebRequest.Get($"{baseVehicleRequestsUri}{endpoint}");

            www.timeout = RequestTimeout;

            www.SetRequestHeader("User-Agent", "Experiment with Tesla API");

            www.SetRequestHeader("Authorization", $"{Authentication.token_type} {Authentication.access_token}");

            www.SendWebRequest();

            while (!www.isDone) {

                await Task.Delay(100);
            }

            if (DebugLevel == LogLevel.Http) Debug.Log("GET: " + www.downloadHandler.text);


            return JsonConvert.DeserializeObject<T>(FormatJSON(www.downloadHandler.text));
        }

        /// <summary>
        /// POST request for Tesla API.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        private async Task<T> PostRequestAsync<T>(string endpoint, WWWForm form) {

            using UnityWebRequest www = UnityWebRequest.Post($"{baseVehicleRequestsUri}{endpoint}", form);

            www.timeout = RequestTimeout;

            www.SetRequestHeader("User-Agent", "Experiment with Tesla API");

            www.SetRequestHeader("Authorization", $"{Authentication.token_type} {Authentication.access_token}");

            www.SendWebRequest();

            while (!www.isDone) {

                await Task.Delay(100);
            }

            if (DebugLevel == LogLevel.Http) Debug.Log("POST: " + www.downloadHandler.text);

            return JsonConvert.DeserializeObject<T>(FormatJSON(www.downloadHandler.text));
        }

        /// <summary>
        /// Oauth for getting the owner api access token for making vehicle endpoint requests to the Tesla API.
        /// </summary>
        /// <returns></returns>
        private async Task<Authentication> OAuthRequestsAsync(string email, string password) {

            HttpClientHandler httpClientHandler = new HttpClientHandler {

                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,

                AllowAutoRedirect = false,

                UseCookies = true
            };

            client = new HttpClient(httpClientHandler) {

                DefaultRequestHeaders =
                {
                    Connection = { "keep-alive" },

                    Accept = { new MediaTypeWithQualityHeaderValue("application/json") },

                    UserAgent = {ProductInfoHeaderValue.Parse("TeslaClientManager/1.0") }
                }
            };

            // Step 1: Obtain the login page https://tesla-api.timdorr.com/api-basics/authentication#step-1-obtain-the-login-page

            string code_verifier = GenerateRandomString(86);

            string challenge = GenerateChallenge(code_verifier);

            string state = GenerateRandomString(32);

            Dictionary<string, string> authCodeRequestUrlParams = new Dictionary<string, string>() {

                {"client_id", "ownerapi"},

                {"code_challenge", challenge},

                {"code_challenge_method","S256"},

                {"redirect_uri", redirectUri},

                {"response_type","code"},

                {"scope","openid email offline_access"},

                {"state", state}
            };

            string combinedAuthCodeRequestUrlParams = string.Join("&", authCodeRequestUrlParams.Select(kvp => $"{HttpUtility.UrlPathEncode(kvp.Key)}={HttpUtility.UrlPathEncode(kvp.Value)}"));

            string authCodeRequestFullUri = $"{authCodeRequestBaseEndpoint}?{combinedAuthCodeRequestUrlParams}";

            if (DebugLevel == LogLevel.Http) Debug.Log(authCodeRequestFullUri);

            Dictionary<string, string> formInputElementValues = new Dictionary<string, string>();

            HttpResponseMessage loginPageRequestResponse = await client.GetAsync(new Uri(authCodeRequestFullUri));

            string sid = loginPageRequestResponse.Headers.GetValues("set-cookie").First().Split(' ').First();

            HttpContent loginPageRequestContent = loginPageRequestResponse.Content;

            string loginPageRequestHtml = loginPageRequestContent.ReadAsStringAsync().Result;

            for (int i = 0; i < hiddenInputFormElements.Length; i++) {

                string match = Regex.Match(loginPageRequestHtml, $"<input type=\"hidden\" name=\"{hiddenInputFormElements[i]}\" value=\"(.*?)\"").Groups[1].Value;

                formInputElementValues.Add(hiddenInputFormElements[i], match);
            }

            formInputElementValues.Add("identity", email);

            formInputElementValues.Add("credential", password);

            // Step 2: Obtain an authorization code https://tesla-api.timdorr.com/api-basics/authentication#step-2-obtain-an-authorization-code

            HttpContent authCodeRequestContent = new FormUrlEncodedContent(formInputElementValues);

            authCodeRequestContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded") { CharSet = "UTF-8" };

            HttpRequestMessage authCodeRequestMessage = new HttpRequestMessage(HttpMethod.Post, authCodeRequestFullUri) { Content = authCodeRequestContent };

            authCodeRequestMessage.Headers.Add("Cookie", sid);

            HttpResponseMessage authCodeRequestResponse = await client.SendAsync(authCodeRequestMessage);

            string locationHeader = authCodeRequestResponse.Headers.GetValues("location").First();

            Uri locationHeaderUri = new Uri(locationHeader);

            string authCode = HttpUtility.ParseQueryString(locationHeaderUri.Query).Get("code");

            if (DebugLevel == LogLevel.Http) Debug.Log("Auth Code From Callback Url: " + authCode);

            // Step 3: Exchange authorization code for bearer token https://tesla-api.timdorr.com/api-basics/authentication#post-https-auth-tesla-com-oauth-2-v-3-token

            string bearerTokenRequestJson = JsonConvert.SerializeObject(new BearerTokenRequest() {

                grant_type = "authorization_code",

                client_id = "ownerapi",

                code = authCode,

                code_verifier = code_verifier,

                redirect_uri = redirectUri
            });

            if (DebugLevel == LogLevel.Http) Debug.Log("Bearer Token Request JSON: " + bearerTokenRequestJson);

            var bearerTokenRequestContent = new StringContent(bearerTokenRequestJson, Encoding.UTF8, "application/json");

            HttpRequestMessage bearerTokenRequestMessage = new HttpRequestMessage(HttpMethod.Post, bearerTokenRequestEndpoint) { Content = bearerTokenRequestContent };

            HttpResponseMessage bearerTokenRequestResponse = await client.SendAsync(bearerTokenRequestMessage);

            string bearerTokenRequestResponseBodyJson = await bearerTokenRequestResponse.Content.ReadAsStringAsync();

            if (DebugLevel == LogLevel.Http) Debug.Log($"Bearer Token Request Response Body JSON: {bearerTokenRequestResponseBodyJson}");

            BearerTokenRequestResponse bearerResponseBodyObject = JsonConvert.DeserializeObject<BearerTokenRequestResponse>(bearerTokenRequestResponseBodyJson);

            // Step 4: Exchange bearer token for access token https://tesla-api.timdorr.com/api-basics/authentication#step-4-exchange-bearer-token-for-access-token

            string accessTokenExchangeJson = JsonConvert.SerializeObject(new AccessTokenExchangeRequest() {

                grant_type = "urn:ietf:params:oauth:grant-type:jwt-bearer",

                client_id = clientID,

                client_secret = clientSecret
            });

            var accessTokenExchangeRequestContent = new StringContent(accessTokenExchangeJson, Encoding.UTF8, "application/json");

            HttpRequestMessage accessTokenExchangeRequestMessage = new HttpRequestMessage(HttpMethod.Post, accessTokenExchangeEndpoint) { Content = accessTokenExchangeRequestContent };

            accessTokenExchangeRequestMessage.Headers.Add("Authorization", $"{bearerResponseBodyObject.token_type} {bearerResponseBodyObject.access_token}");

            HttpResponseMessage accessTokenExchangeRequestResponse = await client.SendAsync(accessTokenExchangeRequestMessage);

            string accessTokenExchangeRequestResponseJson = await accessTokenExchangeRequestResponse.Content.ReadAsStringAsync();

            if (DebugLevel == LogLevel.Http) Debug.Log($"Access Token Exchange Request Response Json: {accessTokenExchangeRequestResponseJson}");

            return JsonConvert.DeserializeObject<Authentication>(accessTokenExchangeRequestResponseJson);
        }

        /// <summary>
        /// Reset authentication
        /// </summary>
        public void ResetClientCache() {

            if (File.Exists(configPath) && Authentication != null) {

                File.Delete(configPath);

                Authentication = null;

                if (DebugLevel == LogLevel.Client) Debug.Log($"Reset Client Cache: Deleted Authentication Config File");

                OnClientCacheReset?.Invoke();
            }          
        }

        /// <summary>
        /// Generate url-encoded code challenege from code_verifier random string.
        /// </summary>
        /// <returns></returns>
        private string GenerateChallenge(string verifier) {

            return WebUtility.UrlEncode(verifier);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// RNGCryptoServiceProvider to generate random string.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        private string GenerateRandomString(int length) {

            const string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            var res = new StringBuilder(length);

            using (var rng = new RNGCryptoServiceProvider()) {

                int count = (int)Math.Ceiling(Math.Log(alphabet.Length, 2) / 8.0);

                Debug.Assert(count <= sizeof(uint));

                int offset = BitConverter.IsLittleEndian ? 0 : sizeof(uint) - count;

                int max = (int)(Math.Pow(2, count * 8) / alphabet.Length) * alphabet.Length;

                byte[] uintBuffer = new byte[sizeof(uint)];

                while (res.Length < length) {

                    rng.GetBytes(uintBuffer, offset, count);

                    uint num = BitConverter.ToUInt32(uintBuffer, 0);

                    if (num < max) {

                        res.Append(alphabet[(int)(num % alphabet.Length)]);
                    }
                }
            }

            return res.ToString();
        }

        /// <summary>
        /// Format json for newtonsoft json utility.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        private string FormatJSON(string json) {

            if (!json.Contains("response")) {

                return "{" + "\"response\"" + ":" + "[" + json + "]" + "}";

            } else {

                return json;
            }
        }

        #endregion
    }

    /// <summary>
    /// Custom Unity Event for when a new user has been created.
    /// </summary>
    [Serializable] public class UserInitialized : UnityEvent<User> { }

    /// <summary>
    /// Custom Unity Event for when vehicles have retrieved.
    /// </summary>
    [Serializable] public class UserVehiclesRetrieved : UnityEvent<List<Vehicle>> { }

    /// <summary>
    /// Custom Unity Event for when vehicles have awaken.
    /// </summary>
    [Serializable] public class UserVehiclesAwaken : UnityEvent<bool> { }

    /// <summary>
    /// Object for bearer token request.
    /// </summary>
    [Serializable]
    public class BearerTokenRequest {

        public string grant_type { get; set; }

        public string client_id { get; set; }

        public string code { get; set; }

        public string code_verifier { get; set; }

        public string redirect_uri { get; set; }
    }

    /// <summary>
    /// Object response from bearer token request response.
    /// </summary>
    [Serializable]
    public class BearerTokenRequestResponse {

        public string access_token { get; set; }

        public string refresh_token { get; set; }

        public int expires_in { get; set; }

        public string state { get; set; }

        public string token_type { get; set; }
    }

    /// <summary>
    /// Object response from Access token exchange request.
    /// </summary>
    [Serializable]
    public class AccessTokenExchangeRequest {

        public string grant_type { get; set; }

        public string client_id { get; set; }

        public string client_secret { get; set; }
    }

    /// <summary>
    /// Object for refresh token refresh.
    /// </summary>
    [Serializable]
    public class RefreshTokenRequest {

        public string grant_type { get; set; }

        public string client_id { get; set; }

        public string refresh_token { get; set; }

        public string scope { get; set; }
    }
}