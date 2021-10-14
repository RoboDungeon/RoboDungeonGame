public static class NetworkPlayerExtensions
{

    public static void SetSettings( this NetworkPlayer player, NetworkPlayerSettings settings, TeamData data )
    {
        player.SetSettings(
                              settings.MaxHealth,
                              settings.MoveSpeed,
                              settings.WeaponCooldown,
                              settings.BulletDamage,
                              settings.BulletBounces,
                              settings.BulletSpeed,
                              settings.BulletHasMaxTravelTime,
                              settings.BulletMaxTravelTime,
                              settings.BlockingCooldown,
                              settings.BlockingTime,
                              settings.GameStartCountdown,
                              data.TeamColor,
                              data.TeamBulletColor
                             );
    }

}