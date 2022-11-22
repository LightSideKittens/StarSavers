using System.Collections.Generic;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using MusicEventSystem.Configs;
using UnityEditor;
using UnityEngine;

namespace MusicEventSystem.Editor
{
    public class MidiToJsonConverterWindow : EditorWindow
    {
        private string jsonName;
        private int bpm;
        private Object midiFile;

        private Dictionary<FourBitNumber, (string, List<NoteData>)> tracks = new Dictionary<FourBitNumber, (string, List<NoteData>)>();
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
                    MainMusicData.MusicName = jsonName;
                }
                EditorGUILayout.EndHorizontal();
                
                TempoMap tempo;

                if (!Equals(tempMidiFile, midiFile))
                {
                    midiFile = tempMidiFile;
                    tracks.Clear();
                    selectedNames.Clear();
                    MainMusicData.Clear();
                    GetMidiProperties(path, out midi, out tempo, out bpm);
                        
                    var nameIndex = 0;

                    foreach (var channel in midi.GetChannels())
                    {
                        var trackName = $"Channel_{channel}";
                        tracks.Add(channel, (GUILayout.TextField(trackName), new List<NoteData>()));
                        selectedNames.Add(channel, nameIndex);
                        nameIndex++;
                    }
                }
                else
                {
                    GetMidiProperties(path, out midi, out tempo, out _);
                    var tempTracks = new List<(FourBitNumber, string, List<NoteData>)>();
                    var style = new GUIStyle() {alignment = TextAnchor.MiddleCenter};
                    style.normal.textColor = Color.white;
                    style.fontSize = 20;
                    style.richText = true;
                    
                    GUILayout.Label($"<b>Tracks</b>", style);
                        
                    foreach (var track in tracks)
                    {
                        selectedNames[track.Key] = EditorGUILayout.Popup($"Track {track.Key}", selectedNames[track.Key], SoundTypes.Types);
                        tempTracks.Add((track.Key, SoundTypes.Types[selectedNames[track.Key]], track.Value.Item2));
                    }

                    for (int i = 0; i < tempTracks.Count; i++)
                    {
                        var track = tempTracks[i];
                        tracks[track.Item1] = (track.Item2, track.Item3);
                    }
                }
                
                if (midi != null)
                {
                    bpm = EditorGUILayout.IntField("BPM", bpm);
                    
                    EditorGUILayout.Space(10);
                        
                    if (GUILayout.Button("Convert to JSON"))
                    {
                        foreach (var note in midi.GetNotes())
                        {
                            float time = note.TimeAs<MetricTimeSpan>(tempo).TotalMicroseconds / 1000000f;
                            float length = note.LengthAs<MetricTimeSpan>(tempo).TotalMicroseconds / 1000000f;
                            
                            tracks[note.Channel].Item2.Add(new NoteData(time, time + length));
                        }

                        foreach (var track in tracks.Values)
                        {
                            MainMusicData.SetTrack(track.Item1, track.Item2.ToArray());
                        }
            
                        MainMusicData.MusicName = jsonName;
                        MainMusicData.BPM = bpm;
                        MainMusicData.SaveAsDefault();
                        AssetDatabase.Refresh();
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void GetMidiProperties(string path, out MidiFile midi, out TempoMap tempo, out int bpm)
        {
            midi = MidiFile.Read(path);
            tempo = midi.GetTempoMap();
            bpm = (int)tempo.Tempo.AtTime(0).BeatsPerMinute;
        }
    }
}