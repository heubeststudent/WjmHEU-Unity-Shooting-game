using UnityEngine;

public class Enemy : Character
{
    [SerializeField] int deathEnergyBonus = 5;
    [SerializeField] int scorePoint = 100;
    [SerializeField] protected int healthFactor = 2;
    protected override void OnEnable()
    {
        SetHealth();
        base.OnEnable();
    }
    protected virtual  void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.Die();
            Die();
        }
    }
    public override void Die()
    {
        ScoreManager.Instance.AddScore(scorePoint);
        PlayerEnergy.Instance.Obtain(deathEnergyBonus);
        EnemyManager.Instance.RemoveFromList(gameObject);
        base.Die();
    }

    protected virtual void SetHealth()
    {
        maxHealth += (int)(EnemyManager.Instance.WaveNumber / healthFactor);
    }
}
