using System.IO;
using UnityEditor;
using UnityEngine;


public class MusicDataGenerator
{
    [MenuItem("MusicDataGenerator/Generate")]
    public static void Generate()
    {
        var files = Directory.GetFiles($"{Application.dataPath}/MusicsData/", "*.asset");
        
        for (int i = 0; i < files.Length; i++)
        {
            var path = files[i].Substring(Application.dataPath.Length - 6);
            var rhythm = AssetDatabase.LoadAssetAtPath<Rhythm>(path);
            
            var layers = rhythm.layers;

            for (int j = 0; j < layers.Count; j++)
            {
                var layer = layers[j];
                var markups = layer.markups;
                var notes = new NoteData[markups.Count];

                for (int k = 0; k < markups.Count; k++)
                {
                    var markup = markups[k];
                    notes[k] = new NoteData(markup.Timer, markup.Timer + markup.Length);
                }
                
                MusicData.CreateTrack(layer.layerName, notes);
            }
            
            MusicData.musicName = rhythm.name;
            MusicData.Save();
        }
    }
}
