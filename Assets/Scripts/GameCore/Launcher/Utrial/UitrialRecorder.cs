/**
 * UitrialRecorder is a product of Uitrial LTD and released under MIT license.
 * Feel free to update the code logic for your needs.
 * Some WWWForm fields are required for this script to work.
 */

using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Launcher;
using UnityEngine.Serialization;

namespace Uitrial
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Networking;

    public static class Manager
    {
        public static bool IsInited { get; private set; }
        private static GameObject gameObject;
        private static Recorder recorder;

        public static void SetupAndStartRecording(string apiKey)
        {
            gameObject = new GameObject("UitrialRecorder");
            recorder = gameObject.AddComponent<Recorder>();
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            recorder.apiKey = apiKey;

            IsInited = true;
        }

        public static void PauseRecording()
        {
            recorder.Pause();
        }

        public static void ResumeRecording()
        {
            recorder.Resume();
        }

        public static bool IsRecording()
        {
            return recorder.IsRecording();
        }

        public static void Destroy()
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }

    public class Recorder : MonoBehaviour
    {
        private string apiUrl = "https://gs.uitrial.com/api";
        private string ipAddress = "18.159.59.46";
        public string apiKey { get; set; }

        private bool isRecording = true;
        private string sessionUuid;
        private byte[] sessionUuidBytes;
        [SerializeField] private int fps = 12;
        private float time;
        private float timeThreshold;

        public string installReferrer = "";
        public string installReferrerSource = "";
        public string playerLocalId = "";
        public string playerName = "";
        private UdpClient udpClient;
        
        void Start()
        {
            IPAddress remoteIpAddress = IPAddress.Parse(ipAddress);
            IPEndPoint remoteEndPoint = new IPEndPoint(remoteIpAddress, 5678);
            udpClient = new UdpClient();
            udpClient.Connect(remoteEndPoint);
            timeThreshold = 1f / fps;
            
            StartCoroutine(StartSession());
        }

        void Update()
        {
            if (CanRecord())
            {
                time += Time.deltaTime;
                
                if (time >= timeThreshold)
                {
                    time = 0;
                    StartCoroutine(CaptureFrame());
                }
            }
        }

        IEnumerator CaptureFrame()
        {
            yield return new WaitForEndOfFrame();

            byte[] jpg;

            if (isRecording)
            {
                var texture = ScreenCapture.CaptureScreenshotAsTexture();
                var encoder = new JPGEncoder(texture, 50);

                yield return new WaitEncode(encoder);
                
                jpg = encoder.GetBytes();
                //File.WriteAllBytes($"{Application.streamingAssetsPath}/Test/Frame_{currentFrame}.jpg", jpg);
                Destroy(texture);
            }
            else
            {
                Texture2D blackTexture = new Texture2D(Screen.width, Screen.height);
                Color32[] blackPixels = new Color32[blackTexture.width * blackTexture.height];

                for (int i = 0; i < blackPixels.Length; i++)
                {
                    blackPixels[i] = Color.black;
                }

                blackTexture.SetPixels32(blackPixels);

                blackTexture.Apply();
                jpg = blackTexture.EncodeToJPG();
                Destroy(blackTexture);
            }
            
            byte[] result = new byte[65510];
            /*Buffer.BlockCopy(sessionUuidBytes, 0, result, 0, sessionUuidBytes.Length);
            Buffer.BlockCopy(jpg, 0, result, sessionUuidBytes.Length, jpg.Length);*/
            
            /*using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[MaxDatagramSize - 4];
                int bytesRead = 0;
                int sequenceNumber = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    byte[] sequenceNumberBytes = BitConverter.GetBytes(sequenceNumber);
                    byte[] datagram = new byte[bytesRead + 4];
                    sequenceNumberBytes.CopyTo(datagram, 0);
                    buffer.CopyTo(datagram, 4);
                    udpClient.Send(datagram, datagram.Length);
                    sequenceNumber++;
                }
            }
            
            udpClient.Send(result, result.Length);*/
        }

        IEnumerator StartSession()
        {
            WWWForm form = new WWWForm();

            form.AddField("start_timestamp", DateTime.UtcNow.ToString());
            form.AddField("timezone", "Europe/Zurich");
            form.AddField("player[name]", playerName);
            form.AddField("player[local_id]", playerLocalId);
            form.AddField("player[device_model]", SystemInfo.deviceModel);
            form.AddField("player[device_type]", SystemInfo.deviceType.ToString());
            form.AddField("player[device_os]", SystemInfo.operatingSystem);
            form.AddField("player[device_unique_id]", SystemInfo.deviceUniqueIdentifier);
            form.AddField("fps", 12);

            UnityWebRequest www = UnityWebRequest.Post($"{apiUrl}/sessions", form);
            www.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            www.SetRequestHeader("Accept", $"application/json");

            www.downloadHandler = new DownloadHandlerBuffer();
            Debug.Log($"[Malvis] {www.uri}");
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string response = www.downloadHandler.text;
                Session session = JsonUtility.FromJson<Session>(response);
                sessionUuid = session.uuid;
                Debug.Log($"[Malvis] {www.downloadHandler.text}");
                sessionUuidBytes = Encoding.UTF8.GetBytes(sessionUuid);
                Debug.Log($"[Malvis] Id: {sessionUuid}. Bytes: {sessionUuidBytes.Length}");
            }

            www.Dispose();
        }

        bool CanRecord()
        {
            return isRecording && (! string.IsNullOrEmpty(sessionUuid)) && Application.isPlaying;
        }

        public void Pause()
        {
            isRecording = false;
        }

        public void Resume()
        {
            isRecording = true;
        }

        public bool IsRecording()
        {
            return isRecording;
        }
    }

    [Serializable]
    public class Session
    {
        public string uuid;
    }
}