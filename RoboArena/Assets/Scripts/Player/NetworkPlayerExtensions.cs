public static class NetworkPlayerExtensions
{

    public static void RpcSetSettings( this NetworkPlayer player, NetworkPlayerSettings settings )
    {
        player.RpcSetSettings(
                              settings.MaxHealth,
                              settings.MoveSpeed,
                              settings.WeaponCooldown,
                              settings.BulletDamage,
                              settings.BulletBounces,
                              settings.BulletSpeed,
                              settings.BulletHasMaxTravelTime,
                              settings.BulletMaxTravelTime
                             );
    }

}