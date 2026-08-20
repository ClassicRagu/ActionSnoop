using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility;
using FFXIVClientStructs.FFXIV.Common.Math;
using Dalamud.Bindings.ImGui;
using System.Linq;
using ActionViewer.Models;
using System;

namespace ActionViewer.Windows.Config;

public partial class ConfigWindow
{

    // This file was taken mostly from Diadem Calculator by Infiziert90: https://github.com/Infiziert90/DiademCalculator
    private void FTBConfig()
    {
        if (ImGui.BeginTabItem("FTB"))
        {
            var changed = false;

            var width = ImGui.GetWindowWidth();

            ImGui.Columns(2, "Job Columns", false);

            
            foreach (var ocjob in Plugin.Configuration.SHJobs.Select((value, i) => (value, i)))
            {
                ImGui.SetNextItemWidth(width / 3f);
                if (ImGui.InputInt(PJobMappings.pJobList[ocjob.i], ref Plugin.Configuration.SHJobs[ocjob.i], 1))
                {
                    Plugin.Configuration.SHJobs[ocjob.i] = Math.Clamp(Plugin.Configuration.SHJobs[ocjob.i], 0, 48);
                    changed = true;
                }
                ImGui.NextColumn();
                changed |= ImGui.Checkbox($"{PJobMappings.pJobList[ocjob.i]} Allow Greater?", ref Plugin.Configuration.SHJobOverflow[ocjob.i]);
                ImGui.NextColumn();
            }

            if (changed)
                Plugin.Configuration.Save();

            ImGui.EndTabItem();
        }
    }
}
