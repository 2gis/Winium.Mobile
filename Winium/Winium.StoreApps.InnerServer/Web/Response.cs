// <copyright file="Response.cs" company="Salesforce.com">
//
// Copyright (c) 2014 Salesforce.com, Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the
// following conditions are met:
//
//    Redistributions of source code must retain the above copyright notice, this list of conditions and the following
//    disclaimer.
//
//    Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the
//    following disclaimer in the documentation and/or other materials provided with the distribution.
//
//    Neither the name of Salesforce.com nor the names of its contributors may be used to endorse or promote products
//    derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>

namespace Winium.StoreApps.InnerServer.Web
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using Winium.Mobile.Common;
    using Winium.StoreApps.InnerServer;

    /// <summary>
    /// Represents a command response.
    /// </summary>
    public class Response
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
