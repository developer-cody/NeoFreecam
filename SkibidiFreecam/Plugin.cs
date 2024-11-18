using System;
using BepInEx;
using UnityEngine;
using Utilla;

namespace SkibidiFreecam
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public GameObject Head;
        public GameObject HandL;
        public GameObject HandR;

        void Start()
        {
            GorillaTagger.OnPlayerSpawned(OnGameInitialized);
        }

        void OnEnable()
        {
            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            HarmonyPatches.RemoveHarmonyPatches();
        }

        void OnGameInitialized()
        {
            Head = new GameObject();
            HandL = new GameObject();
            HandR = new GameObject();

            HandL.transform.parent = Camera.main.transform;
            HandR.transform.parent = Camera.main.transform;
            HandleRigCONST();
        }

        public void HandleRigCONST()
        {
            GorillaTagger.Instance.offlineVRRig.head.overrideTarget = Head.transform;
            GorillaTagger.Instance.offlineVRRig.leftHand.overrideTarget = HandL.transform;
            GorillaTagger.Instance.offlineVRRig.rightHand.overrideTarget = HandR.transform;
        }

        void Update()
        {
          
        }
    }
}
