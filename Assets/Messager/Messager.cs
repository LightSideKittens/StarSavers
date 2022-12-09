using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using Core.Extensions.Unity;
using Core.ReferenceFrom.Extensions.Unity;
using Firebase.Extensions;
using Firebase.Firestore;
using GameCore.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace BeatRoyale
{
    public class Messager : MonoBehaviour
    {
        [SerializeField] private ScrollRect messageParent;

        [Serializable]
        private class MessageData
        {
            [SerializeField] private LayoutGroup messageTemplate;
            [ReferenceFrom(nameof(messageTemplate)), SerializeField] private LayoutGroup[] layoutGroups;
            [ReferenceFrom(nameof(messageTemplate)), SerializeField] private ProceduralImage back;
            [ReferenceFrom(nameof(messageTemplate)), SerializeField] private Image userIcon;
            [ReferenceFrom(nameof(messageTemplate)), SerializeField] private TMP_Text userName;
            [ReferenceFrom(nameof(messageTemplate)), SerializeField] private TMP_Text messageText;
            
            private ContentSizeFitter content;
            private ScrollRect scrollRect;
            private static Dictionary<string, Color> userColorsMap = new Dictionary<string, Color>();
            private static Dictionary<string, Sprite> userSpritesMap = new Dictionary<string, Sprite>();
            private Sprite[] userSprites;
            private bool isRightAligned;

            public void Init(ScrollRect scrollRect, Sprite[] userSprites, ContentSizeFitter content, bool isRightAligned)
            {
                this.scrollRect = scrollRect;
                this.userSprites = userSprites;
                this.isRightAligned = isRightAligned;
                this.content = content;
            }

            public void Create(string userId, string message)
            {
                var instance = Instantiate(messageTemplate, content.transform);

                new CountDownTimer(1, true).Stopped += () =>
                {
                    var transform = instance.transform;
                    transform.SetParent(scrollRect.content.transform);
                    
                    Destroy(instance);
                    
                    for (int i = 0; i < layoutGroups.Length; i++)
                    {
                        var layoutGroup = layoutGroups[i];
                        Destroy(transform.Get(layoutGroup));
                    }
                };

                var userName = instance.Get(this.userName);
                var messageText = instance.Get(this.messageText);
                var userIcon = instance.Get(this.userIcon);
                var back = instance.Get(this.back);

                if (isRightAligned)
                {
                    userIcon.transform.parent.transform.SetAsLastSibling();
                }

                if (userColorsMap.TryGetValue(userId, out var color) == false)
                {
                    color = Colors[userColorsMap.Count % Colors.Length].color;
                    userColorsMap.Add(userId, color);
                }
                
                if (userSpritesMap.TryGetValue(userId, out var sprite) == false)
                {
                    sprite = userSprites[userSpritesMap.Count % userSprites.Length];
                    userSpritesMap.Add(userId, sprite);
                }

                back.color = color;
                userIcon.sprite = sprite;
                
                userName.text = userId;
                messageText.text = message;
                scrollRect.verticalNormalizedPosition = 0;
                messagesCount++;
            }
        }
        
        public struct ColorData
        {
            public Color color;

            public ColorData(Color color, float brightFactor = 0.8f, float saturationFactor = 0.5f)
            {
                this.color = color.CloneAndChangeSaturation(saturationFactor);
                this.color = this.color.CloneAndChangeBrightness(brightFactor);
            }
        }

        public static ColorData[] Colors { get; } = new[]
        {
            new ColorData(Color.yellow),
            new ColorData(Color.blue),
            new ColorData(Color.green),
            new ColorData(Color.cyan),
            new ColorData(Color.magenta, 1, 0.3f),
            new ColorData(Color.gray),
            new ColorData(Color.white),
        };

        [ColoredField, SerializeField] private MessageData leftAlignMessage;
        [ColoredField, SerializeField] private MessageData rightAlignMessage;
        
        [SerializeField] private TMP_InputField messageField;
        [SerializeField] private Button sendButton; 
        [SerializeField] private ContentSizeFitter contentSizeFitter; 
        [SerializeField] private Sprite[] userSprites; 
        private Dictionary<string, object> messages;
        private static int messagesCount;
        private string userId;
        private CollectionReference messagesRef;
        private ListenerRegistration messagesListener;
        
        public void OnLogin(string userId)
        {
            gameObject.SetActive(true);
            leftAlignMessage.Init(messageParent, userSprites, contentSizeFitter, false);
            rightAlignMessage.Init(messageParent, userSprites, contentSizeFitter, true);
            this.userId = userId;
            sendButton.AddListener(SendMessage);
            messagesRef = FirebaseFirestore.DefaultInstance
                .Collection("messager")
                .Document("messages")
                .Collection($"messages_{DateTime.Now:yyyy-MM-dd}");
            
            messagesListener = messagesRef.Listen(CreateAllMessages);
        }

        private void CreateAllMessages(QuerySnapshot snapshot)
        {
            var documents = snapshot.Documents.ToList();

            if (documents.Count > messagesCount)
            {
                for (int i = messagesCount; i < documents.Count; i++)
                {
                    var messageDict = documents[i].ToDictionary();

                    if (messageDict.ContainsKey(userId))
                    {
                        rightAlignMessage.Create(userId, (string) messageDict[userId]);
                    }
                    else
                    {
                        using var messagePair = messageDict.GetEnumerator();
                        messagePair.MoveNext();
                        leftAlignMessage.Create(messagePair.Current.Key, (string) messagePair.Current.Value);
                    }
                }
            }

            Debug.Log("Read all data from the users collection.");
        }

        private void OnDestroy()
        {
            messagesListener.Stop();
        }

        private async void SendMessage()
        {
            rightAlignMessage.Create(userId, messageField.text);
            var docRef = messagesRef.Document($"message_{DateTime.Now:HH-mm-ss}");
            
            var message = new Dictionary<string, object>
            {
                {$"{userId}", messageField.text},
            };

            messageField.text = string.Empty;
            
            await docRef.SetAsync(message).ContinueWithOnMainThread(task => {
                Debug.Log("Message sent.");
            });
        }
    }
}
