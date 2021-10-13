using System;

using UnityEngine;

[Serializable]
public class NetworkPlayerSettings
{

    [Header("Player Settings")]
    [SerializeField]
    private int m_MaxHealth = 100;
    [SerializeField]
    private float m_MoveSpeed = 2000;

    [Header("Weapon Settings")]
    [SerializeField]
    private float m_WeaponCooldown = 0.4f;
    [Header("Bullet Settings")]
    [SerializeField]
    private int m_BulletBounces = 15;
    [SerializeField]
    private int m_BulletDamage= 5;
    [SerializeField]
    private float m_BulletSpeed = 15;
    [SerializeField]
    private bool m_BulletHasMaxTravelTime = false;
    [SerializeField]
    private float m_BulletMaxTravelTime = 10;

    public float WeaponCooldown
    {
        get => m_WeaponCooldown;
        set => m_WeaponCooldown = value;
    }

    public float MoveSpeed
    {
        get => m_MoveSpeed;
        set => m_MoveSpeed = value;
    }

    public int MaxHealth
    {
        get => m_MaxHealth;
        set => m_MaxHealth = value;
    }

    public int BulletBounces
    {
        get => m_BulletBounces;
        set => m_BulletBounces = value;
    }

    public int BulletDamage
    {
        get => m_BulletDamage;
        set => m_BulletDamage = value;
    }

    public float BulletSpeed
    {
        get => m_BulletSpeed;
        set => m_BulletSpeed = value;
    }

    public bool BulletHasMaxTravelTime
    {
        get => m_BulletHasMaxTravelTime;
        set => m_BulletHasMaxTravelTime = value;
    }

    public float BulletMaxTravelTime
    {
        get => m_BulletMaxTravelTime;
        set => m_BulletMaxTravelTime = value;
    }

}