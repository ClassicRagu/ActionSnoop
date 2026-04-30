using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace ActionViewer
{
    public sealed class Services
    {
        public static void Initialize(IDalamudPluginInterface pluginInterface) => pluginInterface.Create<Services>();

        [PluginService]
        public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;

        [PluginService]
        public static ICommandManager Commands { get; private set; } = null!;

        [PluginService]
        public static IObjectTable Objects { get; private set; } = null!;

        [PluginService]
        public static IClientState ClientState { get; private set; } = null!;

        [PluginService]
        public static IObjectTable ObjectTable { get; private set; } = null!;

        [PluginService]
        public static IChatGui Chat { get; private set; } = null!;
        [PluginService]
        public static IDataManager DataManager { get; private set; } = null;
    }
}
