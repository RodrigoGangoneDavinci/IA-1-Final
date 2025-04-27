using UnityEngine;

public class LeaderTwo : Leader
{
    void Update()
    {
        base.Update();

        //TODO : crear validacion para que no se pueda hacer click si el current State es Scape o Death
        if (Input.GetMouseButtonDown(1))
        {
            TrySetDestination();
        }
        
        //TODO: DEBUG
        if (Input.GetKeyDown(KeyCode.F))
        {
            TakeDamage(25f);
        }
    }
}