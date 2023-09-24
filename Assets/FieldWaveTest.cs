using System;
using DG.Tweening;
using UnityEngine;

namespace BeatRoyale
{
    public class FieldWaveTest : MonoBehaviour
    {
        private SpriteRenderer[] cells;

        private void Start()
        {
            cells = GetComponentsInChildren<SpriteRenderer>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var index = 0;
                
                for (int i = 0; i < cells.Length; i++)
                {
                    var bounds = cells[i].bounds;
                    var size = bounds.size;
                    size.z = 0;
                    bounds.size = size;
                    var min = bounds.min;
                    var max = bounds.max;
                    
                    if (position.x > min.x && position.y > min.y
                        && position.x < max.x && position.y < max.y)
                    {
                        index = i;
                        break;
                    }
                }

                Animate(index, () =>
                {
                    Animate(index - 1);
                    Animate(index + 1);
                    
                    index += 18;
                    
                    Animate(index - 1);
                    Animate(index);
                    
                    index -= 18;
                    index -= 18;
                    Animate(index - 1);
                    Animate(index);
                });
                
                void Animate(int k, Action onComplete = null)
                {
                    var cellTransform = cells[k].transform;
                    cellTransform.DOScale(Vector3.one * 0.9f, 0.1f).OnComplete(() =>
                    {
                        onComplete?.Invoke();
                        cellTransform.DOScale(Vector3.one, 1f);
                    });
                }
            }
        }
    }
}
