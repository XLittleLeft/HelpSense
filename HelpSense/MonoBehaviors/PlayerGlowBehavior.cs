using AdminToys;
using HelpSense.Helper;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using PlayerRoles;
using UnityEngine;
using LightSourceToy = AdminToys.LightSourceToy;

namespace HelpSense.MonoBehaviors
{
    public class PlayerGlowBehavior : MonoBehaviour
    {
        private Player _player;
        private LightSourceToy _glowLight;
        private bool flag = true;
        void Awake()
        {
            _player = Player.Get(gameObject);
        }

        void Start()
        {
            _glowLight = Instantiate(LightBaseObject);
            _glowLight.transform.position = _player.Position;
            _glowLight.transform.eulerAngles = Vector3.zero;
            _glowLight.transform.localScale = Vector3.zero;
            NetworkServer.Spawn(_glowLight.gameObject);
            _glowLight.NetworkLightColor = Color.yellow;
            //_glowLight.NetworkLightShadows = true;
            _glowLight.NetworkLightRange = 2f;
            _glowLight.NetworkLightIntensity = 1f;
        }

        void Update()
        {
            if (_player == null || _glowLight == null)
            {
                Destroy(this);
                return;
            }

            if (_glowLight.NetworkPosition == _player.Position + Vector3.up * 0.6f) return;

            _glowLight.NetworkPosition = _player.Position + Vector3.up * 0.6f;
            _glowLight.UpdatePositionClient();
        }

        void FixedUpdate()
        {
            if (flag && _player != null)
            {
                flag = false;
                Timing.CallDelayed(1f, () =>
                {
                    flag = true;
                });
                foreach (Player Player in XHelper.PlayerList)
                {
                    if (Vector3.Distance(Player.Position, _player.Position) <= 1 && Player != _player && Player.Role is not RoleTypeId.Scp079)
                    {
                        Player?.Damage(1f, "受到SCP-1093的辐射伤害");
                    }
                }
            }
        }

        void OnDestroy()
        {
            NetworkServer.UnSpawn(_glowLight.gameObject);
            NetworkServer.Destroy(_glowLight.gameObject);
            _glowLight = null;
        }

        public static LightSourceToy LightBaseObject
        {
            get
            {
                foreach (GameObject gameObject in NetworkClient.prefabs.Values)
                {
                    if (gameObject.TryGetComponent(out LightSourceToy component))
                    {
                        return component;
                    }
                }
                return null;
            }
        }
    }
}