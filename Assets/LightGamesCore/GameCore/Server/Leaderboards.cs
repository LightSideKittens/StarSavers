using System;
using System.Collections.Generic;
using System.Linq;
using Core.ConfigModule;
using Firebase.Extensions;
using Firebase.Firestore;
using Newtonsoft.Json;

namespace Core.Server
{
    public class Leaderboards : JsonBaseConfigData<Leaderboards>
    {
        [JsonProperty] private int rank;
        [JsonProperty] private long position;
        private static CollectionReference reference;

        public static int Rank
        {
            get => Config.rank;
            set
            {
                Config.rank = value;
                OnRankUpdated();
            }
        }

        public static long Position
        {
            get => Config.position;
            set => Config.position = value;
        }

        protected override void OnLoading()
        {
            base.OnLoading();
            reference = User.Database.Collection("Leaderboards");
            var configReference = User.Database
                .Collection("PlayersData")
                .Document(User.Id)
                .Collection("Data")
                .Document(FileName);
            configReference.Listen(OnChanged);
        }

        private void OnChanged(DocumentSnapshot snapshot)
        {
            var configDict = snapshot.ToDictionary();

            if (configDict is {Count: > 0})
            {
                var json = (string)configDict[Config.FileName];
                var config = JsonConvert.DeserializeObject<Leaderboards>(json);
                position = config.position;
                rank = config.rank;
            }
        }

        private static void RunTransaction(DocumentReference reference, Action<DocumentSnapshot> onSuccess, Action onError)
        {
            User.Database.RunTransactionAsync(transaction =>
            {
                return transaction.GetSnapshotAsync(reference).ContinueWithOnMainThread(snapshot =>
                {
                    if (snapshot.IsCompletedSuccessfully)
                    {
                        onSuccess.SafeInvoke(snapshot.Result);
                    }
                    else
                    {
                        onError.SafeInvoke();
                    }
                });
            });
        }

        private static void OnRankUpdated()
        {
            Burger.Log($"[{nameof(Leaderboards)}] OnRankUpdated");
            
            if (Position == 0)
            {
                User.GetPlayersCount((_, count) =>
                {
                    Position = count;
                    SetPosition(count);
                });
            }
            else if (Rank == 0)
            {
                SetDefaultPosition();
            }
            else if (Position > 1)
            {
                UpdatePosition();
            }
            else if (Position == 1)
            {
                SetPosition(1);
            }
        }

        public static void SetPosition(long position)
        {
            Burger.Log($"[{nameof(Leaderboards)}] SetPosition: {position}");
            
            var positionRef = reference.Document($"{position}");
            positionRef.SetAsync(Data.Create(User.Id, Rank));
        }
        
        private static void SetDefaultPosition()
        {
            SetPosition(Position);
        }

        private static void UpdatePosition()
        {
            Burger.Log($"[{nameof(Leaderboards)}] UpdatePosition. Current: {Position}");
            
            var targetPositionRef = reference.Document($"{Position - 1}");
            var batch = User.Database.StartBatch();
            
            RunTransaction(targetPositionRef, snapshot =>
            {
                var targetDict = snapshot.ToDictionary();
                var targetPair = targetDict.ElementAt(0);
                Burger.Log($"[{nameof(Leaderboards)}] Target: UserId: {targetPair.Key} Position: {targetPair.Value}");
                
                if(Rank > (long)targetPair.Value)
                {
                    Burger.Log($"[{nameof(Leaderboards)}] Start Swapping!");
                    targetDict.Clear();
                    targetDict[User.Id] = Rank;
                    var selfPositionRef = reference.Document($"{Position}");
                    
                    RunTransaction(selfPositionRef, selfSnapshot =>
                    {
                        var selfDict = selfSnapshot.ToDictionary();
                        var selfPair = selfDict.ElementAt(0);
                        var leaderboardConfigRef = User.Database
                            .Collection("PlayersData")
                            .Document(targetPair.Key)
                            .Collection("Data")
                            .Document(Config.FileName);

                        RunTransaction(leaderboardConfigRef, configSnapshot =>
                        {
                            var configDict = configSnapshot.ToDictionary();
                            var json = (string)configDict[Config.FileName];
                            var config = JsonConvert.DeserializeObject<Leaderboards>(json);
                            config.position++;
                            json = JsonConvert.SerializeObject(config, Formatting.None);
                            configDict[Config.FileName] = json;
                            batch.Set(leaderboardConfigRef, configDict);

                            if (selfPair.Key == User.Id)
                            {
                                selfDict.Clear();
                                selfDict[targetPair.Key] = targetPair.Value;
                                batch.Set(selfPositionRef, selfDict);
                            }
                            
                            batch.Set(targetPositionRef, targetDict);
                            batch.CommitAsync().ContinueWithOnMainThread(task =>
                            {
                                if (task.IsCompletedSuccessfully)
                                {
                                    Position--;
                                    Burger.Log($"[{nameof(Leaderboards)}] Success Swapped!");
                                }
                                else
                                {
                                    Burger.Error($"[{nameof(Leaderboards)}] Failure Swapped");
                                }
                            });

                        }, SetDefaultPosition);
                    }, SetDefaultPosition);
                }
            }, SetDefaultPosition);
        }
    }
}