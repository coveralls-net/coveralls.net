using System;
using Newtonsoft.Json;

namespace Coveralls
{
    public class UriJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Uri);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null) return null;

            switch (reader.TokenType)
            {
                case JsonToken.String:
                    return CreateUri((string)reader.Value);
                case JsonToken.Null:
                    return null;
                default:
                    var msg = string.Format("Unable to deserialize Uri from token type {0}", reader.TokenType);
                    throw new InvalidOperationException(msg);
            }
        }

        private static Uri CreateUri(string url)
        {
            Uri uri = null;
            if (uri == null) Uri.TryCreate(url, UriKind.Absolute, out uri);
            if (uri == null) Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri);

            if (uri == null)
            {
                var message = string.Format("Unable to determine proper format for Uri {0}", url);
                throw new InvalidOperationException(message);
            }

            return uri;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null) return;

            var uri = value as Uri;
            if (uri == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteValue(uri.OriginalString);
            }
        }
    }
}