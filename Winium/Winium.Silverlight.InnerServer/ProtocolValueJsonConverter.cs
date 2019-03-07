// <copyright file="ProtocolValueJsonConverter.cs" company="Salesforce.com">
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

namespace Winium.Silverlight.InnerServer
{
    using System;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    /// <summary>
    /// Converts the response to JSON
    /// </summary>
    internal class ProtocolValueJsonConverter : JsonConverter
    {
        /// <summary>
        /// Checks if the object can be converted
        /// </summary>
        /// <param name="objectType">The object to be converted</param>
        /// <returns>True if it can be converted or false if can't be</returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        /// <summary>
        /// Process the reader to return an object from JSON
        /// </summary>
        /// <param name="reader">A JSON reader</param>
        /// <param name="objectType">Type of the object</param>
        /// <param name="existingValue">The existing value of the object</param>
        /// <param name="serializer">JSON Serializer</param>
        /// <returns>Object created from JSON</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return this.ProcessToken(reader);
        }

        /// <summary>
        /// Writes objects to JSON. Currently not implemented
        /// </summary>
        /// <param name="writer">JSON Writer Object</param>
        /// <param name="value">Value to be written</param>
        /// <param name="serializer">JSON Serializer </param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (serializer != null)
            {
                serializer.Serialize(writer, value);
            }
        }

        private object ProcessToken(JsonReader reader)
        {
            // Recursively processes a token. This is required for elements that next other elements.
            object processedObject = null;
            if (reader != null)
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    Dictionary<string, object> dictionaryValue = new Dictionary<string, object>();
                    while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                    {
                        string elementKey = reader.Value.ToString();
                        reader.Read();
                        dictionaryValue.Add(elementKey, this.ProcessToken(reader));
                    }

                    processedObject = dictionaryValue;
                }
                else if (reader.TokenType == JsonToken.StartArray)
                {
                    List<object> arrayValue = new List<object>();
                    while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                    {
                        arrayValue.Add(this.ProcessToken(reader));
                    }

                    processedObject = arrayValue.ToArray();
                }
                else
                {
                    processedObject = reader.Value;
                }
            }

            return processedObject;
        }
    }
}
