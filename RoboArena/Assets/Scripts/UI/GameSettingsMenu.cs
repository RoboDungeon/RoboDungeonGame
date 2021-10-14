using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{

    public class GameSettingsMenu : MonoBehaviour
    {

        private int m_CurrentMode;

        private GameLogic CurrentMode => m_Modes[m_CurrentMode];
        [SerializeField]
        private GameLogic[] m_Modes;
        [SerializeField]
        private string[] m_Maps;

        [SerializeField]
        private Dropdown m_ModeDropdown;
        [SerializeField]
        private Dropdown m_MapDropdown;
        [SerializeField]
        private InputField m_PlayerCount;
        [SerializeField]
        private InputField m_PlayerHealth;
        [SerializeField]
        private InputField m_MoveSpeed;
        [SerializeField]
        private InputField m_BlockingTime;
        [SerializeField]
        private InputField m_BlockingCooldown;
        [SerializeField]
        private InputField m_WeaponCooldown;
        [SerializeField]
        private InputField m_BulletBounces;
        [SerializeField]
        private InputField m_BulletDamage;
        [SerializeField]
        private InputField m_BulletSpeed;
        [SerializeField]
        private InputField m_BulletMaxTime;

        private void Start()
        {
            PopulateDropdowns();
            SetModeData( 0 );
        }

        public void SetModeData(int m)
        {
            m_CurrentMode = m;
            m_ModeDropdown.captionText.text = CurrentMode.Name;
            PopulateModeSettings();
        }

        public void SetMapData(int m)
        {
            m_MapDropdown.captionText.text = m_Maps[m];
            PopulateModeSettings();
        }

        private void PopulateModeSettings()
        {
            m_PlayerCount.text = ArenaSceneData.Instance.StartGameAtPlayerCount.ToString();
            m_PlayerHealth.text = CurrentMode.PlayerSettings.MaxHealth.ToString();
            m_MoveSpeed.text = CurrentMode.PlayerSettings.MoveSpeed.ToString();
            m_BlockingTime.text = CurrentMode.PlayerSettings.BlockingTime.ToString();
            m_BlockingCooldown.text = CurrentMode.PlayerSettings.BlockingCooldown.ToString();
            m_WeaponCooldown.text = CurrentMode.PlayerSettings.WeaponCooldown.ToString();
            m_BulletBounces.text = CurrentMode.PlayerSettings.BulletBounces.ToString();
            m_BulletDamage.text = CurrentMode.PlayerSettings.BulletDamage.ToString();
            m_BulletSpeed.text = CurrentMode.PlayerSettings.BulletSpeed.ToString();
            m_BulletMaxTime.text = CurrentMode.PlayerSettings.BulletMaxTravelTime.ToString();
        }

        public void SaveSettings()
        {
            CurrentMode.PlayerSettings.MaxHealth = int.Parse(m_PlayerHealth.text);
            CurrentMode.PlayerSettings.BulletSpeed = float.Parse(m_MoveSpeed.text);
            CurrentMode.PlayerSettings.BlockingTime = float.Parse(m_BlockingTime.text);
            CurrentMode.PlayerSettings.BlockingCooldown = float.Parse(m_BlockingCooldown.text);
            CurrentMode.PlayerSettings.WeaponCooldown = float.Parse(m_WeaponCooldown.text);
            CurrentMode.PlayerSettings.BulletBounces = int.Parse(m_BulletBounces.text);
            CurrentMode.PlayerSettings.BulletDamage = int.Parse(m_BulletDamage.text);
            CurrentMode.PlayerSettings.BulletSpeed = float.Parse(m_BulletSpeed.text);
            CurrentMode.PlayerSettings.BulletMaxTravelTime = float.Parse(m_BulletMaxTime.text);
            ArenaSceneData.Instance.StartGameAtPlayerCount = int.Parse(m_PlayerCount.text);
            ArenaSceneData.Instance.MapName = m_Maps[m_MapDropdown.value];
            ArenaSceneData.Instance.GameMode = m_Modes[m_CurrentMode];
        }

        private void PopulateDropdowns()
        {
            m_MapDropdown.options.Clear();
            foreach (string map in m_Maps)
            {
                m_MapDropdown.options.Add(new Dropdown.OptionData(map));
            }

            SetMapData( 0 );
            m_ModeDropdown.options.Clear();
            foreach (GameLogic mode in m_Modes)
            {
                m_ModeDropdown.options.Add(new Dropdown.OptionData(mode.Name));
            }
            SetModeData(0);
        }

    }

}
