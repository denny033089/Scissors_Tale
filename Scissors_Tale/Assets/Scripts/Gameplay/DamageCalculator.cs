using UnityEngine;

public class DamageCalculator : Singleton<DamageCalculator>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CalculateDamage(Piece piece,int damage) {
        if(piece is Player player) {
            player.TakeDamage(damage);
        }
    }
}
