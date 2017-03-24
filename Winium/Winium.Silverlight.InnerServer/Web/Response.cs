namespace Winium.Silverlight.InnerServer.Web
{

    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Winium.Mobile.Common;

    class Response
    {
        /// <summary>
        /// Gets or sets the status of the response.
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public ResponseStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the session ID of the command.
        /// </summary>
        [JsonProperty("sessionId", NullValueHandling = NullValueHandling.Ignore)]
        public string SessionId { get; set; }

        /// <summary>
        /// Gets or sets the value of the response.
        /// </summary>
        [JsonProperty(PropertyName = "value", NullValueHandling = NullValueHandling.Include)]
        [JsonConverter(typeof(ProtocolValueJsonConverter))]
        public object Value { get; set; }

        /// <summary>
        /// Creates a response from a serialized JSON string.
        /// </summary>
        /// <param name="jsonResponse">The serialized JSON string containing the response.</param>
        /// <returns>The response object.</returns>
        public static Response FromJson(string jsonResponse)
        {
            Response response = JsonConvert.DeserializeObject<Response>(jsonResponse);
            return response;
        }

        /// <summary>
        /// Creates a success response.
        /// </summary>
        /// <param name="responseValue">The value contained by the response.</param>
        /// <returns>The response indicating a successful command execution.</returns>
        public static Response CreateSuccessResponse(object responseValue = null)
        {
            Response response = new Response();
            if (responseValue == null)
            {
                responseValue = new Dictionary<string, object>();
            }
            else
            {
                response.Value = responseValue;
            }

            return response;
        }

        /// <summary>
        /// Creates an error response with the specified error code and message.
        /// </summary>
        /// <param name="errorCode">The error code of the response.</param>
        /// <param name="errorMessage">The error message containing information about the error.</param>
        /// <returns>The response indicating an unsuccessful command execution.</returns>
        public static Response CreateErrorResponse(ResponseStatus errorCode, string errorMessage)
        {
            Response response = new Response();
            response.Status = errorCode;
            Dictionary<string, object> errorDetails = new Dictionary<string, object>();
            errorDetails["message"] = errorMessage;
            response.Value = errorDetails;
            return response;
        }

        /// <summary>
        /// Creates an error response when a command is missing values for its parameters.
        /// </summary>
        /// <param name="missingParameters">The list of missing parameters.</param>
        /// <returns>The response indicating an unsuccessful command execution.</returns>
        public static Response CreateMissingParametersResponse(string missingParameters)
        {
            Response response = new Response();
            response.Status = ResponseStatus.MissingParameters;
            response.Value = missingParameters;
            return response;
        }

        /// <summary>
        /// Serializes this <see cref="Response"/> to JSON.
        /// </summary>
        /// <returns>The JSON-serialized response.</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
