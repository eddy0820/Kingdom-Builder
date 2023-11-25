using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public List<InteractionTypeSO> InteractionTypes { get; }
    public IDamageable IDamageable { get; }
}
