using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Core.ConfigModule.Converters
{
    public class BoundsJsonConverter : JsonConverter<Bounds>
    {
        public override Bounds ReadJson(JsonReader reader, Type objectType, Bounds existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            Bounds col = new Bounds();

            JObject data = JObject.Load(reader);
            col.center = data["center"]!.ToObject<Vector3>();
            col.extents = data["extents"]!.ToObject<Vector3>();
            col.size = data["size"]!.ToObject<Vector3>();
            col.min = data["min"]!.ToObject<Vector3>();
            col.max = data["max"]!.ToObject<Vector3>();

            return col;
        }

        public override void WriteJson(JsonWriter writer, Bounds value, JsonSerializer serializer)
        {

            JObject col = new JObject()
            {
                ["center"] = ToJObject(value.center),
                ["extents"] = ToJObject(value.extents),
                ["size"] = ToJObject(value.size),
                ["min"] = ToJObject(value.min),
                ["max"] = ToJObject(value.max)
            };

            col.WriteTo(writer);
        }

        private JObject ToJObject(Vector3 vector)
        {
            JObject pos = new JObject()
            {
                ["x"] = vector.x,
                ["y"] = vector.y,
                ["z"] = vector.z
            };

            return pos;
        }
    }
}