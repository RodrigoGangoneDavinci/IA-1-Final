using UnityEngine;

public class LeaderOne : Leader
{
    void Update()
    {
        base.Update();

        //TODO : crear validacion para que no se pueda hacer click si el current State es Scape o Death
        if (Input.GetMouseButtonDown(0)) 
        {
            TrySetDestination();
        }
        
        //TODO: DEBUG
        if (Input.GetKeyDown(KeyCode.D))
        {
            TakeDamage(25f);
        }
    }
}