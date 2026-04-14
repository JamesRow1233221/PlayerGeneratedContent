using UnityEngine;

abstract public class PowerUp
{
    CarController carController;
    bool carSet = false;

    public void SetCarController(CarController car)
    {
        carController = car;
        carSet = true;
    }

    abstract public void Use();
    abstract public void Update(float delta);
    abstract public void FixedUpdate(float delta);
}
