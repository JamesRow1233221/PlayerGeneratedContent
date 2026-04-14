using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    PowerUp currentPowerUp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void CollectPowerUp(PowerUp powerUp)
    {
        currentPowerUp = powerUp;
    }

    public void UsePowerUp()
    {
        if (currentPowerUp == null) return;

        currentPowerUp.Use();
    }

    public void ClearPowerUp()
    {
        currentPowerUp = null;
    }
}
