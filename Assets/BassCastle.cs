using DefaultNamespace;
using DefaultNamespace.MusicEventSystem;
using UnityEngine;

public class BassCastle : Castle
{
    protected override void OnDestroyEnemy(string soundType, GameObject enemy)
    {
        var bullet = Instantiate(EnemyBulletPairs.Pairs[soundType].bullet, transform.position, Quaternion.identity);
        var sine = bullet.GetComponent<Sinewave>();
        sine.SetTarget(soundType, enemy.transform);
        sine.NoteEnded += () =>
        {
            Instantiate(GetFX(soundType), enemy.transform.position, Quaternion.identity);
            Destroy(enemy.gameObject);
        };
    }
}
