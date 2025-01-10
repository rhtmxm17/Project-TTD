using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageAddable
{
    public void IDamageAdd(float damage);
}

public interface IProgressable
{ 
    public void IPrograssable(Combatable monster);
}
