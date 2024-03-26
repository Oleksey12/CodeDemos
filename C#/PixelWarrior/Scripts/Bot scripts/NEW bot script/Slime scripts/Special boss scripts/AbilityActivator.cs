using System.Collections.Generic;
using UnityEngine;
/*
 * Скрипт управлят использованием рандомных способностей боссом
 */
public class AbilityActivator : MonoBehaviour, IAbilityActivator
{
    private IList<IBossAbility> _abilities;

    public IBossAbility _currentAbility;


    private void Start()
    {
        _abilities = new List<IBossAbility>(GetComponents<IBossAbility>());
        changeAbility();
    }
    private void changeAbility()
    {
        _currentAbility = _abilities[Random.Range(0, _abilities.Count)];
    }

    public void ActivateCurrentAbility()
    {
        changeAbility();
        _currentAbility.Activate();
    }



    public bool GetAbilityState()
    {
        return _currentAbility._isAbilityCasting;
    }
}

