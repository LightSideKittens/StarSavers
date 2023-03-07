using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace FastSpriteMask
{
    public static class MultiMaskUtility
    {
        private static readonly int MaskTexId = Shader.PropertyToID("_MaskTex");
        private static readonly int MaskDataId = Shader.PropertyToID("_MaskData");

        private const int HARD_MASK_DATA_STEP = 7;
        private const int SOFT_MASK_DATA_STEP = 6;

        public static (float[] data, List<int> rectCounter, Texture2D dataTexture, List<IMultiMask> masks) CreateMaskData(int resolution = 256)
        {
            var data = new float[resolution];
            var rectCounter = new List<int>(resolution >> 3);

            data[0] = 1;
            return (data, rectCounter, new Texture2D(resolution >> 2, 1, GraphicsFormat.R32G32B32A32_SFloat, TextureCreationFlags.None)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
            }, new List<IMultiMask>(rectCounter.Count));
        }

        public static void Expand(ref float[] data, Texture2D texture, int rectCount)
        {
            var length = data.Length;
            var newLenght = length << 1;
            var newData = new float[newLenght];

            var leftEnd = (int)data[0];
            var rightShift = rectCount << 2;

            Array.Copy(data, newData, leftEnd + 1);
            Array.ConstrainedCopy(data, length - rightShift, newData, newLenght - rightShift, rightShift);

            data = newData;
            texture.Reinitialize(newLenght >> 2, 1);
            texture.Apply();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetMaskTexture(this Material material, IMultiMask mask)
        {
            material.SetTexture(MaskTexId, mask.Sprite.texture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetMaskTexture(this Material material, Texture2D dataTexture)
        {
            material.SetTexture(MaskTexId, dataTexture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetMaskTexture(this Material material, Sprite sprite)
        {
            material.SetTexture(MaskTexId, sprite.texture);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetMaskData(this Material material, Texture2D dataTexture, float[] data)
        {
            dataTexture.SetPixelData(data, 0);
            dataTexture.Apply();

            material.SetTexture(MaskDataId, dataTexture);
        }

        /// <summary>
        /// Reserve multiMask data in data array.
        /// </summary>
        /// <param name="masks"> The list of masks </param>
        /// <param name="data"> The data of masks that will be sent to the Material </param>
        /// <param name="rectCounter"> Includes counts of masks that reference to the certain rect </param>
        /// <param name="multiMask"> The sprite of a multiMask </param>
        /// <returns> DataIndex for IMultiMask. If it is -1 then addition was failure </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AddMask(this List<IMultiMask> masks, float[] data, List<int> rectCounter, IMultiMask multiMask)
        {
            if (!AddMask(data, rectCounter, multiMask)) return false;

            masks.Add(multiMask);

            return true;
        }

        /// <summary>
        /// Reserve multiMask data in data array.
        /// </summary>
        /// <param name="data"> The data of masks that will be sent to the Material </param>
        /// <param name="rectCounter"> Includes counts of masks that reference to the certain rect </param>
        /// <param name="multiMask"> The sprite of a multiMask </param>
        /// <returns> DataIndex for IMultiMask. If it is -1 then addition was failure </returns>
        public static bool AddMask(float[] data, List<int> rectCounter, IMultiMask multiMask)
        {
            var dataStep = multiMask.Type.IsHard() ? HARD_MASK_DATA_STEP : SOFT_MASK_DATA_STEP;

            var dataLength = data.Length;
            var rectCount = rectCounter.Count;

            var leftEnd = (int)data[0];
            var rightEnd = dataLength - (rectCount << 2);

            var delta = rightEnd - leftEnd;
            if (delta < dataStep) return false;

            var sprite = multiMask.Sprite;
            var rectMin = sprite.textureRect.min;
            var rectMax = sprite.textureRect.max;

            for (var rectIndex = rightEnd; rectIndex < dataLength; rectIndex += 4)
            {
                if (data[rectIndex] == rectMin.x && data[rectIndex + 1] == rectMin.y && data[rectIndex + 2] == rectMax.x && data[rectIndex + 3] == rectMax.y)
                {
                    multiMask.DataIndex = leftEnd;

                    var counterId = ((dataLength - rectIndex) >> 2) - 1;

                    rectCounter[counterId]++;

                    data[leftEnd + 5] = counterId + 1; // Set rectId
                    data[0] = leftEnd + dataStep;

                    return true;
                }
            }

            if (delta < dataStep + 4) return false;
            else
            {
                multiMask.DataIndex = leftEnd;

                rectCounter.Add(1); // The count of the rect users;

                data[--rightEnd] = rectMax.y;
                data[--rightEnd] = rectMax.x;
                data[--rightEnd] = rectMin.y;
                data[--rightEnd] = rectMin.x;

                data[leftEnd + 5] = rectCount + 1; // Set rectId
                data[0] = leftEnd + dataStep;

                return true;
            }
        }

        /// <summary>
        /// Remove multiMask from multiMask list and update data.
        /// </summary>
        /// <param name="masks"> The multiMask list </param>
        /// <param name="data"> The data of masks that will be sent to the Material </param>
        /// <param name="rectCounter"> Includes counts of masks that reference to the certain rect </param>
        /// <param name="multiMask"> The multiMask that should be removed. Be sure that multiMask belongs to multiMask list </param>
        public static void RemoveMask(this List<IMultiMask> masks, float[] data, List<int> rectCounter, IMultiMask multiMask)
        {
            var dataStep = multiMask.Type.IsHard() ? HARD_MASK_DATA_STEP : SOFT_MASK_DATA_STEP;

            var dataIndex = multiMask.DataIndex; // Index of multiMask data
            var index = (dataIndex - 1) / dataStep; // Index of multiMask
            masks.RemoveAt(index); // Remove multiMask from list

            // Shift data indexes in following masks
            for (var i = index; i < masks.Count; i++)
            {
                masks[i].DataIndex -= dataStep;
            }

            var rectId = (int)data[dataIndex + 5];
            var counterId = rectId - 1;

            var leftEnd = (int)data[0];
            var fromIndex = dataIndex + dataStep;
            Array.ConstrainedCopy(data, fromIndex, data, dataIndex, leftEnd - fromIndex);

            data[0] = leftEnd -= dataStep; // Now the multiMask data length is less by 6

            // Check if there is any users of the rect
            if (--rectCounter[counterId] > 0) return;

            var lastRectId = rectCounter.Count;
            // If removed rect isn't last
            if (rectId != lastRectId)
            {
                var dataLength = data.Length;
                var lastRectIndex = dataLength - (lastRectId << 2);
                var destinationRectIndex = dataLength - (rectId << 2);

                Array.ConstrainedCopy(data, lastRectIndex, data, destinationRectIndex, 4);

                var lastCounterId = lastRectId - 1;
                rectCounter[counterId] = rectCounter[lastCounterId];

                for (int i = 6, c = rectCounter[counterId]; i <= leftEnd; i += dataStep)
                {
                    // If multiMask's rect index is the index of shifted rect
                    if ((int)data[i] == lastRectId)
                    {
                        // Set new index of rect
                        data[i] = rectId;
                        // If user count will be 0
                        if (c == 1) break;
                        c--;
                    }
                }
            }

            rectCounter.RemoveAt(lastRectId - 1);
        }

        /// <summary>
        /// Set transform and alpha cutoff data of masks.
        /// </summary>
        /// <param name="masks"> The multiMask list </param>
        /// <param name="data"> The data of masks that will be sent to the Material </param>
        /// <param name="isHard"> Are masks a hard </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateState(this List<IMultiMask> masks, float[] data, bool isHard)
        {
            if (isHard)
            {
                for (var i = 0; i < masks.Count; i++)
                {
                    var di = masks[i].DataIndex;
                    var tr = masks[i].Transform;

                    var pos = tr.position;
                    var scale = ((Vector2) tr.lossyScale) / masks[i].Sprite.pixelsPerUnit;
                    var rad = tr.rotation.eulerAngles.z * Mathf.Deg2Rad;

                    data[di] = pos.x;
                    data[di + 1] = pos.y;
                    data[di + 2] = rad;
                    data[di + 3] = scale.x;
                    data[di + 4] = scale.y;

                    data[di + 6] = masks[i].AlphaCutoff;
                }
            }
            else
            {
                UpdateTransform(masks, data);
            }
        }


        /// <summary>
        /// Set transform data of masks.
        /// </summary>
        /// <param name="masks"> The multiMask list </param>
        /// <param name="data"> The data of masks that will be sent to the Material </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateTransform(this List<IMultiMask> masks, float[] data)
        {
            for (var i = 0; i < masks.Count; i++)
            {
                var di = masks[i].DataIndex;
                var tr = masks[i].Transform;

                var pos = tr.position;
                var scale = ((Vector2) tr.lossyScale) / masks[i].Sprite.pixelsPerUnit;
                var rad = tr.rotation.eulerAngles.z * Mathf.Deg2Rad;

                data[di++] = pos.x;
                data[di++] = pos.y;
                data[di++] = rad;
                data[di++] = scale.x;
                data[di] = scale.y;
            }
        }

        /// <summary>
        /// Set position data of masks.
        /// </summary>
        /// <param name="masks"> The multiMask list </param>
        /// <param name="data"> The data of masks that will be sent to the Material </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdatePosition(this List<IMultiMask> masks, float[] data)
        {
            for (var i = 0; i < masks.Count; i++)
            {
                var di = masks[i].DataIndex;

                var pos = masks[i].Transform.position;

                data[di++] = pos.x;
                data[di] = pos.y;
            }
        }

        /// <summary>
        /// Set scale data of masks.
        /// </summary>
        /// <param name="masks"> The multiMask list </param>
        /// <param name="data"> The data of masks that will be sent to the Material </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateScale(this List<IMultiMask> masks, float[] data)
        {
            for (var i = 0; i < masks.Count; i++)
            {
                var di = masks[i].DataIndex + 3;
                var scale = ((Vector2) masks[i].Transform.lossyScale) / masks[i].Sprite.pixelsPerUnit;

                data[di++] = scale.x;
                data[di]   = scale.y;
            }
        }

        /// <summary>
        /// Set rotation data of masks.
        /// </summary>
        /// <param name="masks"> The multiMask list </param>
        /// <param name="data"> The data of masks that will be sent to the Material </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateRotation(this List<IMultiMask> masks, float[] data)
        {
            for (var i = 0; i < masks.Count; i++)
            {
                data[masks[i].DataIndex + 2] = masks[i].Transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            }
        }

        /// <summary>
        /// Set alpha cutoff data of masks. Make sure that masks are hard!
        /// </summary>
        /// <param name="masks"> The multiMask list </param>
        /// <param name="data"> The data of masks that will be sent to the Material </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UpdateAlphaCutoff(this List<IMultiMask> masks, float[] data)
        {
            for (var i = 0; i < masks.Count; i++)
            {
                data[masks[i].DataIndex + 6] = masks[i].AlphaCutoff;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsHard(this MaskType type)
        {
            return (int)type < 4;
        }
    }
}