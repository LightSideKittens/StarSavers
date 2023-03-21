using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShortNoteTrack = MusicEventSystem.Configs.MusicData.NoteMusicData<ShortNoteTrackData>;

namespace MusicEventSystem.Configs
{
    public class ShortNoteConverter : JsonConverter<ShortNoteTrack>
    {
        public override void WriteJson(JsonWriter writer, ShortNoteTrack value, JsonSerializer serializer)
        {
            var obj = new JObject();
            var tracks = new JObject();
            obj["shortTracks"] = tracks;

            foreach (var track in value.tracks)
            {
                var arr = new JArray();
                tracks[track.Key] = arr;
                var notes = track.Value.notes;
                for (int i = 0; i < notes.Length; i++)
                {
                    arr.Add(notes[i].startTime);
                }
            }
            
            obj.WriteTo(writer);
        }

        public override ShortNoteTrack ReadJson(JsonReader reader, Type objectType, ShortNoteTrack existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            existingValue = new ShortNoteTrack();
            
            JObject obj = JObject.Load(reader);
            var prop = obj.Property("shortTracks");

            foreach (var track in (JObject)prop.Value)
            {
                var notes = new List<ShortNoteData>();
                var arr = (JArray)track.Value;
                var noteData = new ShortNoteData();

                for (int i = 0; i < arr.Count; i++)
                {
                    var jvalue = (JValue) arr[i];
                    var time = (float)Convert.ChangeType(jvalue.Value, typeof(float), CultureInfo.InvariantCulture);
                    noteData.startTime = time;
                    notes.Add(noteData);
                }

                var trackData = new ShortNoteTrackData(track.Key, notes.ToArray());
                existingValue.tracks.Add(track.Key, trackData);
            }
            
            return existingValue;
        }
    }
}