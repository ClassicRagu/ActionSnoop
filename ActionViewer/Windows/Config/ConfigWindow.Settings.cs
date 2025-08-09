using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Common.Math;
using Dalamud.Bindings.ImGui;

namespace ActionViewer.Windows.Config;

public partial class ConfigWindow
{

    // This file was taken mostly from Diadem Calculator by Infiziert90: https://github.com/Infiziert90/DiademCalculator
    private void Settings()
    {
        if (ImGui.BeginTabItem("Settings"))
        {
            var changed = false;

            var width = ImGui.GetWindowWidth();
            ImGui.SetNextItemWidth(width / 2f);
            changed |= ImGui.Checkbox("Anonymous Mode", ref Plugin.Configuration.AnonymousMode);
			changed |= ImGui.Checkbox("Tooltips", ref Plugin.Configuration.Tooltips);
			changed |= ImGui.Checkbox("Target Range Limit", ref Plugin.Configuration.TargetRangeLimit);

			if (changed)
                Plugin.Configuration.Save();

            ImGui.EndTabItem();
        }
    }
}
