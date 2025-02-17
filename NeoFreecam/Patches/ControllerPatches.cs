using HarmonyLib;

namespace NeoFreecam.Patches
{
    [HarmonyPatch(typeof(ControllerInputPoller), "Update")]
    public class ControllerPatches
    {
        public static bool pressed, islefthand;
        public static string fingers;

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