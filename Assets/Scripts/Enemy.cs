using UnityEngine;

public class Enemy : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public float redColorDuration = 0.5f;

    public float CurrentTimeInGame;
    public float lastTimewasDamaged;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Update()
    {
        ChnageIfNeededtocolor();
    }

    public void ChnageIfNeededtocolor()
    {
        CurrentTimeInGame = Time.time;

        if(CurrentTimeInGame >= lastTimewasDamaged + redColorDuration)
        {
            TurnWhite();
        }
    }

    public void TakeDamage()
    {
        spriteRenderer.color = Color.red;
        lastTimewasDamaged = Time.time;
   
    }

    public void TurnWhite()
    {
        spriteRenderer.color = Color.white;
    }
}
    