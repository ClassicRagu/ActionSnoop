using ActionViewer.Tabs;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Numerics;

namespace ActionViewer.Windows.Config;


public partial class ConfigWindow : Window
{
    // This file was taken mostly from Diadem Calculator by Infiziert90: https://github.com/Infiziert90/DiademCalculator
    private Plugin Plugin;

    public ConfigWindow(Plugin plugin) : base("Configuration##ActionViewer")
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(300, 200),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };

        Plugin = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        if (ImGui.BeginTabBar("##ConfigTabBar"))
        {
            Settings();

            About();
        }
        ImGui.EndTabBar();
    }
}
