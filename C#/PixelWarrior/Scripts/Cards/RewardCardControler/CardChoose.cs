using UnityEngine;

/*
 * Скрипт управляет выбором игроком карт
 */
public class CardChoose : MonoBehaviour, ICardChoose
{
    private PlayerData _playerData;
    private PlayerHealthScript _playerHealth;
    [SerializeField] private Canvas _parentCanv;

    private void Start()
    {
        // Хешируем нужные скрипты
        _playerData = FindObjectOfType<PlayerData>();
        _playerHealth = FindObjectOfType<PlayerHealthScript>();
    }



    // При выборе награды игрок получает её характеристики и выбор заканчивается
    public void OnCardChoose(CardScriptableObject stats)
    {
        SetCardValues(stats);

        DestroyParentCanvas();
    }
    protected void SetCardValues(CardScriptableObject stats)
    {
        // Лечим игрока при выборе карты изменяющей здоровье
        _playerHealth.setHealth(_playerHealth.Current_health + _playerHealth.Max_health * (stats.Hp - 1) * stats.MaxHp,
            _playerHealth.Max_health * stats.MaxHp);
        // Устанавливаем остальные параметры из объекта
        _playerData.Damage *= stats.Dmg;
        _playerData.DashPower *= stats.Dash;
        _playerData.DashCooldown = (int)(_playerData.DashCooldown / stats.Cooldown);
        _playerData.AttackCooldown = (int)(_playerData.AttackCooldown / stats.Dmg_cooldown);
        _playerData.Speed *= stats.Speed;

    }

    protected void DestroyParentCanvas()
    {
        _parentCanv.enabled = false;
    }

}