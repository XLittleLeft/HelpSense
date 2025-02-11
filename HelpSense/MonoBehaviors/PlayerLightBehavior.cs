using AdminToys;
using Mirror;
using LabApi.Features.Wrappers;
using UnityEngine;

namespace HelpSense.MonoBehaviors
{
    public class PlayerLightBehavior : MonoBehaviour
    {
        private Player _player;
        private LightSourceToy _glowLight;
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
            _glowLight.NetworkLightColor = Color.red;
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

            if (_glowLight.NetworkPosition == _player.Position) return;

            _glowLight.NetworkPosition = _player.Position + Vector3.up * 0.6f;
            _glowLight.UpdatePositionClient();
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