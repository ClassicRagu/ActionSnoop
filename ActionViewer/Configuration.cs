using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace ActionViewer
{
    [Serializable]
    public class Configuration : IPluginConfiguration
    {
        public int Version { get; set; } = 0;

		public bool Tooltips = true;
		public bool AnonymousMode = false;
        public bool TargetRangeLimit = true;

        public bool UnrestrictZones = false;

		// the below exist just to make saving less cumbersome

		[NonSerialized]
        private IDalamudPluginInterface? pluginInterface;

        public void Initialize(IDalamudPluginInterface pluginInterface)
        {
            this.pluginInterface = pluginInterface;
        }

        public void Save()
        {
            this.pluginInterface!.SavePluginConfig(this);
        }
    }
}
