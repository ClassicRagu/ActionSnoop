using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Common.Math;
using Dalamud.Bindings.ImGui;

namespace ActionViewer.Windows.Config;

public partial class ConfigWindow
{
    // This file was taken mostly from Diadem Calculator by Infiziert90: https://github.com/Infiziert90/DiademCalculator
    private static void About()
    {
        if (ImGui.BeginTabItem("About"))
        {
            var buttonHeight = ImGui.CalcTextSize("RRRR").Y + (20.0f * ImGuiHelpers.GlobalScale);
            if (ImGui.BeginChild("AboutContent", new Vector2(0, -buttonHeight)))
            {
                ImGuiHelpers.ScaledDummy(5.0f);

                ImGui.TextUnformatted("Authors:");
                ImGui.SameLine();
                ImGui.TextColored(ImGuiColors.ParsedPink, Plugin.Authors);

                ImGui.TextUnformatted("Version:");
                ImGui.SameLine();
                // Hardcoded temporarily for API 10 changes
                ImGui.TextColored(ImGuiColors.ParsedOrange, "0.2.0.0");
            }
            ImGui.EndChild();

            ImGui.Separator();
            ImGuiHelpers.ScaledDummy(1.0f);

            if (ImGui.BeginChild("AboutBottomBar", new Vector2(0, 0), false, 0))
            {
                ImGui.PushStyleColor(ImGuiCol.Button, ImGuiColors.DPSRed);
                if (ImGui.Button("Issues"))
                    Dalamud.Utility.Util.OpenLink("https://github.com/boco-bot/ActionViewer/issues");
                ImGui.PopStyleColor();
            }
            ImGui.EndChild();

            ImGui.EndTabItem();
        }
    }
}
