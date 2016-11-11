namespace VRTK
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Class with utility static methods to access Oculus native functions
    /// </summary>
    public class SDK_GearVR_Util
    {
        public enum Bool
        {
            False = 0,
            True
        }
        public enum PlatformUI
        {
            GlobalMenu = 0,
            ConfirmQuit,
            GlobalMenuTutorial,
        }
        [DllImport("OVRPlugin", CallingConvention = CallingConvention.Cdecl)]
        public static extern Bool ovrp_ShowUI(PlatformUI ui);

        public static void ShowGlobalMenu()
        {
            ovrp_ShowUI(PlatformUI.GlobalMenu);
        }

        public static bool ShowConfirmQuit()
        {
            return ovrp_ShowUI(PlatformUI.ConfirmQuit) == Bool.True;
        }

        public static bool ShowGlobalMenuTutorial()
        {
            return ovrp_ShowUI(PlatformUI.GlobalMenuTutorial) == Bool.True;
        }
    }
}
