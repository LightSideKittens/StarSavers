using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.ConfigModule;
using Firebase.Extensions;
using Firebase.Firestore;
using Newtonsoft.Json;
using UnityEngine;

namespace Core.Server
{
    public class Leaderboards : JsonBaseConfigData<Leaderboards>
    
    {
        private enum Result
        {
            Swapped,
            Updated,
            Error,
        }
        
        [JsonProperty] private int rank;
        [JsonProperty] private long position;
        private static CollectionReference reference;
        private static bool isUpdating;
        private static TaskCompletionSource<Result> source = new();

        public static int Rank
        {
            get => Config.rank;
            set
            {
                var diff = value - Config.rank;
                Config.rank = value;
                OnRankUpdated(diff);
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
            var configReference = User.Database
                .Collection("PlayersData")
                .Document(User.Id)
                .Collection("Data")
                .Document(FileName);
            configReference.Listen(OnChanged);
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            reference = User.Database.Collection("Leaderboards");
        }

        private void OnChanged(DocumentSnapshot snapshot)
        {
            Burger.Log($"[{nameof(Leaderboards)}] OnChanged");
            var configDict = snapshot.ToDictionary();

            if (configDict is {Count: > 0})
            {
                var json = (string)configDict[Config.FileName];
                var config = JsonConvert.DeserializeObject<Leaderboards>(json);
                position = config.position;
                rank = config.rank;
            }
        }

        private static void GetSnapshot(Transaction transaction, DocumentReference reference, Action<DocumentSnapshot> onSuccess)
        {
            transaction.GetSnapshotAsync(reference).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    onSuccess.SafeInvoke(task.Result);
                }
                else
                {
                    Burger.Error($"[{nameof(Leaderboards)}] GetSnapshot Error: {task.Exception.Message}");
                    source.SetResult(Result.Error);
                }
            });
        }

        private static void OnRankUpdated(int diff)
        {
            Burger.Log($"[{nameof(Leaderboards)}] OnRankUpdated. Rank: {Rank}");
            if (!User.ServerEnabled)
            {
                return; 
            }
            
            if (Position == 0)
            {
                User.GetPlayersCount(() =>
                {
                    Position = User.PlayersCount;
                    SetPosition();
                });
            }
            else if (Rank == 0)
            {
                SetPosition();
            }
            else if (!isUpdating && diff != 0)
            {
                isUpdating = true;
                User.GetPlayersCount(() =>
                {
                    UpdatePosition(diff);
                }, 0, () => isUpdating = false);
            }
        }

        private static Dictionary<string, object> GetDictFromSnapshot(DocumentSnapshot snapshot, Action<Leaderboards> modify)
        {
            var dict = snapshot.ToDictionary();
            var json = (string) dict[Config.FileName];
            var config = JsonConvert.DeserializeObject<Leaderboards>(json);
            modify(config);
            json = JsonConvert.SerializeObject(config, Formatting.None);
            dict[Config.FileName] = json;

            return dict;
        }

        private static void SetPosition() => Config.Internal_SetPosition();

        private void Internal_SetPosition()
        {
            Burger.Log($"[{nameof(Leaderboards)}] SetPosition: {Position}");
            
            var positionRef = reference.Document($"{Position}");
            positionRef.SetAsync(Data.Create(User.Id, Rank));
        }

        public static void GetUserId(Action<string> userId, int position = 1) => Config.Internal_GetUserId(userId, position);

        private void Internal_GetUserId(Action<string> userId, int position = 1)
        {
            if (!User.ServerEnabled)
            { 
                userId?.Invoke(User.Id);
                return; 
            }

            User.SignIn(() =>
            {
                var positionRef = User.Database.Collection("Leaderboards").Document($"{position}");
                positionRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        var dict = task.Result.ToDictionary();
                        var targetPair = dict.ElementAt(0);
                        userId(targetPair.Key);
                    }
                    else
                    {
                        Burger.Error($"[{nameof(Leaderboards)}] Failure Get UserId: {task.Exception.Message}");
                    }
                });
            });
        }

        private static DocumentReference GetReference(string userId)
        {
            return User.Database
                .Collection("PlayersData")
                .Document(userId)
                .Collection("Data")
                .Document(Config.FileName);
        }

        private static void UpdatePosition(int diff)
        {
            if (!User.ServerEnabled)
            {
                return; 
            }
            
            Burger.Log($"[{nameof(Leaderboards)}] UpdatePosition. Current: {Position}");
            var factor = diff > 0 ? -1 : 1;

            var transactionTask = User.Database.RunTransactionAsync(transaction =>
            {
                source = new TaskCompletionSource<Result>();
                var selfConfigRef = GetReference(User.Id);
                GetSnapshot(transaction, selfConfigRef, selfConfigSnapshot =>
                {
                    var dict = selfConfigSnapshot.ToDictionary();
                    var json = (string) dict[Config.FileName];
                    var selfConfig = JsonConvert.DeserializeObject<Leaderboards>(json);
                    var position = selfConfig.position;
                    var playersCount = User.PlayersCount;

                    if ((playersCount > position && position > 1) || (playersCount == position && factor == -1) || (position == 1 && factor == 1))
                    {
                        var targetPositionRef = reference.Document($"{position + factor}");

                        GetSnapshot(transaction, targetPositionRef, targetSnapshot =>
                        {
                            var targetDict = targetSnapshot.ToDictionary();
                            var targetPair = targetDict.ElementAt(0);
                            var condition = factor == 1 ? Rank < (long)targetPair.Value : Rank > (long)targetPair.Value;
                            
                            Burger.Log($"[{nameof(Leaderboards)}] Target: UserId: {targetPair.Key} Position: {position + factor}");

                            if (condition)
                            {
                                Burger.Log($"[{nameof(Leaderboards)}] Start Swapping!");
                                targetDict.Clear();
                                targetDict[User.Id] = Rank;
                                var targetConfigRef = GetReference(targetPair.Key);

                                GetSnapshot(transaction, targetConfigRef, targetConfigSnapshot =>
                                {
                                    var targetConfigDict = GetDictFromSnapshot(targetConfigSnapshot, config =>
                                    {
                                        config.position -= factor;
                                    });

                                    var selfDict = Data.Create(targetPair.Key, targetPair.Value);
                                    var selfPositionRef = reference.Document($"{position}");

                                    var selfConfigDict = GetDictFromSnapshot(selfConfigSnapshot, config =>
                                    {
                                        config.position += factor;
                                        config.rank = Rank;
                                    });

                                    transaction.Set(selfPositionRef, selfDict);
                                    transaction.Set(targetPositionRef, targetDict);
                                    transaction.Set(targetConfigRef, targetConfigDict);
                                    transaction.Set(selfConfigRef, selfConfigDict);
                                    Burger.Log($"[{nameof(Leaderboards)}] Swapping...");
                                    source.SetResult(Result.Swapped);
                                });
                            }
                            else
                            {
                                Update();
                            }
                        });
                    }
                    else
                    {
                        Update();
                    }

                    void Update()
                    {
                        Burger.Log($"[{nameof(Leaderboards)}] Start Update!");

                        GetSnapshot(transaction, selfConfigRef, snapshot =>
                        {
                            var selfConfigDict = GetDictFromSnapshot(snapshot,
                                config => { config.rank = Rank; });

                            var selfPositionRef = reference.Document($"{position}");
                            var selfDict = Data.Create(User.Id, Rank);

                            transaction.Set(selfPositionRef, selfDict);
                            transaction.Set(selfConfigRef, selfConfigDict);
                            Burger.Log($"[{nameof(Leaderboards)}] Updating...");
                            source.SetResult(Result.Updated);
                        });
                    }
                });
                
                return source.Task;
            });
            
            transactionTask.ContinueWithOnMainThread(_ =>
            {
                isUpdating = false;
                switch (source.Task.Result)
                {
                    case Result.Swapped:
                        Burger.Log($"[{nameof(Leaderboards)}] Success Swapped!");

                        break;
                    case Result.Updated:
                        Burger.Log($"[{nameof(Leaderboards)}] Updated!");

                        break;
                    case Result.Error:
                        Burger.Error($"[{nameof(Leaderboards)}] Error!");

                        break;
                    default:
                        Burger.Error($"[{nameof(Leaderboards)}] Unknown error!");

                        break;
                }
            });
        }
    }
}