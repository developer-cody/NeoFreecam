using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkibidiFreecam.Patches
{
    [HarmonyPatch(typeof(ControllerInputPoller), "Update")]
    public class patchcontrollers
    {
        public static bool pressed;
        public static string fingers;
        public static bool islefthand;

        static void Postfix(ControllerInputPoller __instance)
        {
            if (pressed)
            {


                if (fingers.Contains("Rindex"))
                {
                    __instance.rightControllerIndexFloat = 1f;
                    __instance.rightGrab = true;

                }
                if (fingers.Contains("Rgrip"))
                {
                    __instance.rightControllerGripFloat = 1f;
                    __instance.rightGrab = true;
                }
                if (fingers.Contains("Rthumb"))
                {
                    __instance.rightControllerPrimaryButton = true;
                    __instance.rightGrab = true;
                }

                if (fingers.Contains("Lindex"))
                {
                    __instance.leftControllerIndexFloat = 1f;
                    __instance.leftGrab = true;
                }
                if (fingers.Contains("Lgrip"))
                {
                    __instance.leftControllerGripFloat = 1f;
                    __instance.leftGrab = true;
                }
                if (fingers.Contains("Lthumb"))
                {
                    __instance.leftControllerPrimaryButton = true;
                    __instance.leftGrab = true;
                }



            }

        }
    }
}
