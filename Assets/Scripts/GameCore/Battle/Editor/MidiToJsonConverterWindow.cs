using System.Collections.Generic;
using System.Reflection;
using Battle.MusicEventSystem.Soundvent;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using MusicEventSystem.Configs;
using UnityEditor;
using UnityEngine;
using static SoundventTypes;

namespace MusicEventSystem.Editor
{
    public class MidiToJsonConverterWindow : EditorWindow
    {
        private string jsonName;
        private string[] soundvents;
        private Object midiFile;

        private Dictionary<FourBitNumber, (string, List<LongNoteData>, bool)> tracks = new Dictionary<FourBitNumber, (string, List<LongNoteData>, bool)>();
        private Dictionary<FourBitNumber, int> selectedNames = new Dictionary<FourBitNumber, int>();

        [MenuItem("Window/MIDI to Json Converter")]
        private static void ShowWindow()
        {
            var window = GetWindow<MidiToJsonConverterWindow>();
            window.titleContent = new GUIContent("MIDI to Json Converter");
            window.Show();
        }

        private void OnEnable()
        {
            midiFile = null;
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            {
                var tempMidiFile = EditorGUILayout.ObjectField(midiFile, typeof(Object), true);

                if (tempMidiFile == null)
                {
                    EditorGUILayout.EndVertical();
                    return;
                }

                var path = AssetDatabase.GetAssetPath(tempMidiFile);
                MidiFile midi;

                if (path.EndsWith(".mid") == false)
                {
                    EditorGUILayout.EndVertical();
                    return;
                }
                
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Json Name");
                    jsonName = tempMidiFile.name;
                    jsonName = GUILayout.TextField(jsonName);
                    MusicData.MusicName = jsonName;
                }
                EditorGUILayout.EndHorizontal();
                
                TempoMap tempo;

                if (!Equals(tempMidiFile, midiFile))
                {
                    midiFile = tempMidiFile;
                    tracks.Clear();
                    selectedNames.Clear();
                    MusicData.Clear();
                    GetMidiProperties(path, out midi, out tempo);
                        
                    var nameIndex = 0;

                    foreach (var channel in midi.GetChannels())
                    {
                        var trackName = $"Channel_{channel}";
                        tracks.Add(channel, (GUILayout.TextField(trackName), new List<LongNoteData>(), true));
                        selectedNames.Add(channel, nameIndex);
                        nameIndex++;
                    }
                    
                    var soundventsList = new List<string>();
                    soundventsList.AddRange(Sounds);
                    soundventsList.AddRange(Enemies);
                    soundvents = soundventsList.ToArray();
                }
                else
                {
                    GetMidiProperties(path, out midi, out tempo);
                    var tempTracks = new List<(FourBitNumber, string, List<LongNoteData>, bool)>();
                    var style = new GUIStyle() {alignment = TextAnchor.MiddleCenter};
                    style.normal.textColor = Color.white;
                    style.fontSize = 20;
                    style.richText = true;
                    
                    GUILayout.Label($"<b>Tracks</b>", style);
                    /*style.fontSize = 16;
                    GUILayout.Label($"<b>Tracks</b>", style);*/
                        
                    foreach (var track in tracks)
                    {
                        selectedNames[track.Key] = EditorGUILayout.Popup($"Track {track.Key}", selectedNames[track.Key], soundvents);
                        var soundventType = soundvents[selectedNames[track.Key]];
                        var attribute = typeof(SoundventTypes).GetField(soundventType, BindingFlags.Public | BindingFlags.Static).GetCustomAttribute<SoundventAttribute>();
                        var isShort = attribute.isShort;
                        tempTracks.Add((track.Key, soundvents[selectedNames[track.Key]], track.Value.Item2, isShort));
                    }

                    for (int i = 0; i < tempTracks.Count; i++)
                    {
                        var track = tempTracks[i];
                        tracks[track.Item1] = (track.Item2, track.Item3, track.Item4);
                    }
                }
                
                if (midi != null)
                {
                    EditorGUILayout.Space(10);
                        
                    if (GUILayout.Button("Convert to JSON"))
                    {
                        foreach (var note in midi.GetNotes())
                        {
                            float time = note.TimeAs<MetricTimeSpan>(tempo).TotalMicroseconds / 1000000f;
                            float length = note.LengthAs<MetricTimeSpan>(tempo).TotalMicroseconds / 1000000f;
                            tracks[note.Channel].Item2.Add(new LongNoteData(time, time + length));
                        }

                        foreach (var track in tracks.Values)
                        {
                            if (track.Item3)
                            {
                                var shortNotes = new ShortNoteData[track.Item2.Count];

                                for (int i = 0; i < shortNotes.Length; i++)
                                {
                                    shortNotes[i] = new ShortNoteData(track.Item2[i].startTime);
                                }
                                
                                MusicData.ShortTrackData.SetTrack(track.Item1, new ShortNoteTrackData(track.Item1, shortNotes));
                            }
                            else
                            {
                                MusicData.LongTrackData.SetTrack(track.Item1, new LongNoteTrackData(track.Item1, track.Item2.ToArray()));
                            }
                        }
            
                        MusicData.MusicName = jsonName;
                        MusicData.SaveAsDefault();
                        MusicData.Clear();
                        AssetDatabase.Refresh();
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void GetMidiProperties(string path, out MidiFile midi, out TempoMap tempo)
        {
            midi = MidiFile.Read(path);
            tempo = midi.GetTempoMap();
        }
    }
}