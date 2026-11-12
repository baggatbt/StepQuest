using UnityEngine;

[CreateAssetMenu(menuName = "StepQuest/Buffmage settings")]
public sealed class BuffmageSettings : ScriptableObject
{
    [Header("Physical hero: ATK represents strength; HP represents constitution")]
    public int startingHealth = 28, startingAttack = 6, startingDefense = 1, startingSpeed = 3, startingEnergy = 3;
    public int healthPerLevel = 6, attackPerLevel = 1;
    public float defensePerLevel = .4f;
    public int energyEveryLevels = 5;

    [Header("Fireball? - physical burning book")]
    public ChargeTimingSettings fireballCharge = new ChargeTimingSettings();
    public float fireballDamage = 1.45f, fireballSpeed = .8f;
    public int fireballEnergyCost = 0, fireballEnergyGain = 1, fireballCooldown = 0;

    [Header("Magic Missile - rapid punches")]
    public float barrageSeconds = 2.2f, punchDamage = .43f, minimumPunchInterval = .12f;
    public int maximumPunches = 6, barrageEnergyCost = 2, barrageCooldown = 1;

    [Header("Meteor - become the meteor")]
    public SweepTimingSettings meteorTiming = new SweepTimingSettings();
    public float meteorDamage = 1.65f, meteorPerfectMultiplier = 1.5f, meteorMissMultiplier = .7f;
    public int meteorEnergyCost = 3, meteorCooldown = 2;

    [Header("Sprite animation and pixel VFX")]
    public BuffmageAnimationSet animations;
    public Sprite bookSprite, fireballIcon, punchIcon, meteorIcon, impactSprite;
    [Tooltip("Pixel x, y, width, height within the existing impact sprite sheet.")]
    public Vector4 impactFrame = new Vector4(64, 0, 16, 32);
    public Sprite[] flameFrames;
    public AudioClip hitSound, perfectSound;
    public float throwSeconds = .42f, punchSeconds = .13f, leapSeconds = .55f, leapHeight = 2.5f;

    public static BuffmageSettings Load() => Resources.Load<BuffmageSettings>("Heroes/Buffmage/Settings");
}
