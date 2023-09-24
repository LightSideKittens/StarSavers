using Core.SingleService;

namespace Launcher
{
    public class UitrialTest : SingleService<UitrialTest>
    {
        public string id = "5Pa8Fe8IhPEjXi0EvMdkifuWb3xxCvn3fMGyirnO";
        public bool needStart;

        public static void Begin()
        {
            if (!Uitrial.Manager.IsInited && Instance.needStart)
            {
                DontDestroyOnLoad(Instance);
                Uitrial.Manager.SetupAndStartRecording(apiKey: Instance.id);
            }
        }
    }
}