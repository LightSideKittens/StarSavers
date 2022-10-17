using UnityEngine;

public class SampleListener : RhythmListener {

    public GameObject[] logo;
    public GameObject noteRight;
    public GameObject noteLeft;

    Vector3[] logoScale;

    private void Start()
    {
        MusicData.musicName = "music";
        var kick = MusicData.GetTrack("Kick");
        kick.Started += () => Debug.Log("Kick Start");
        kick.Playing += () => Debug.Log("Kick Play");
        kick.Ended += () => Debug.Log("Kick End");
    }

    private void Update() 
    {
        MusicData.Update(Time.time);
    }

    public override void BPMEvent(RhythmEventData data) {
        logo[2].transform.localScale = logoScale[2] * 1.2f;
    }

    public override void RhythmEvent(RhythmEventData data) {
        if (data.layer.layerName == "h5") {
            if (data.objects.Count > 0) {
                int d = (int)data.objects[0];
                logo[d == 52 ? 0 : 1].transform.localScale = logoScale[d == 52 ? 0 : 1] * 1.2f;
            }
            return;
        }

        float y = 0;
        if (data.objects.Count > 0) {
            y = ((int)data.objects[0] - 65) / 2f;
        }
        bool h1 = (data.layer.layerName == "h1");
        Instantiate(h1 ? noteRight : noteLeft, transform.position + Vector3.up * y + Vector3.right * (h1 ? -8 : 8), Quaternion.identity);
    }


}
