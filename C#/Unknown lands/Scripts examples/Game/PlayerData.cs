using Assets.Project.Scripts.Abstract;
using System.Collections.Generic;

public class PlayerData {
    public readonly Statistics statistics = new Statistics();
    public readonly GameSettings gameSettings = new GameSettings();
    public readonly GameParams gameParams = new GameParams();
    public readonly AttackSettings attackSettings = new AttackSettings();
    public readonly ChargeSettings chargeSettings = new ChargeSettings();
    public readonly Levels levels = new Levels();
}
public class ValueLimits {
    protected KeyValuePair<float, float> invincibility;
    protected KeyValuePair<float, float> healthDelimeter;
    protected KeyValuePair<float, float> maxHealth;
    protected KeyValuePair<float, float> health;
    protected KeyValuePair<float, float> speed;
    protected KeyValuePair<float, float> level;
    protected KeyValuePair<float, float> points;
}
public class Statistics {
    public ValueLimits lims;
    public TimeAlive timeAlive;
    public KillsCount killsCount;

    public class TimeAlive : ProtectedFloatProperty { }
    public class KillsCount : ProtectedIntProperty { }
}
public class GameSettings {
    public ValueLimits lims;
    public InvincibilityTime invincibilityTime;
    public HealthDelimeter healthDelimeter;

    public class InvincibilityTime : ProtectedFloatProperty {
        protected override bool CheckFunction(float newValue) {
            return 0 < newValue && newValue < 1f;
        }
    }
    public class HealthDelimeter : ProtectedIntProperty { }

}
public class GameParams {
    public ValueLimits lims;
    public MaxHealth maxHealth;
    public CurrentHealth currentHealth;
    public Speed speed;
    public CurrentLevel currentLevel;
    public PointsLeft pointsLeft;


    public class MaxHealth : ProtectedIntProperty { }
    public class CurrentHealth : ProtectedIntProperty { }
    public class Speed : ProtectedFloatProperty { }
    public class CurrentLevel : ProtectedIntProperty { }
    public class PointsLeft : ProtectedIntProperty { }
}
public class AttackSettings {
    public ValueLimits lims;

    public AttackName attackName;
    public AttackDamageBoxDist attackDamageBoxDist;
    public AttackSpearDist attackSpearDist;
    public AttackDamage attackDamage;
    public AttackTime attackTime;
    public AttackCooldown attackCooldown;
    public AttackSpeed attackSpeed;
    public AttackKnockback attackKnockback;

    public class AttackName: ProtectedStringProperty { }
    public class AttackDamageBoxDist : ProtectedFloatProperty { }
    public class AttackSpearDist : ProtectedFloatProperty { }
    public class AttackDamage : ProtectedFloatProperty { }
    public class AttackTime : ProtectedFloatProperty { }
    public class AttackCooldown : ProtectedFloatProperty { }
    public class AttackSpeed : ProtectedFloatProperty { }
    public class AttackKnockback : ProtectedFloatProperty { }
}

public class ChargeSettings {
    public ValueLimits lims;
    public ChargeName chargeName;
    public ChargeTime chargeTime;
    public ChargeCooldown chargeCooldown;
    public ChargeSpeed chargeSpeed;
    public ChargeEchoCooldown chargeEchoCooldown;

    public class ChargeName: ProtectedStringProperty { }
    public class ChargeTime : ProtectedFloatProperty { }
    public class ChargeCooldown : ProtectedFloatProperty { }
    public class ChargeSpeed : ProtectedFloatProperty { }
    public class ChargeEchoCooldown : ProtectedFloatProperty { }
}
public class Levels {
    public ValueLimits lims;
    public HealthLevel healthLevel;
    public AttackLevel attackLevel;
    public ChargeLevel chargeLevel;

    public class HealthLevel : ProtectedIntProperty { }
    public class AttackLevel : ProtectedIntProperty { }
    public class ChargeLevel : ProtectedIntProperty { }
}