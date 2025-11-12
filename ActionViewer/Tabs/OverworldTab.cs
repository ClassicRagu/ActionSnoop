using ActionViewer.Functions;
using ActionViewer.Windows;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Types;

namespace ActionViewer.Tabs;

public class OverworldTab : MainWindowTab
{
	public string TabType { get; set; }
	public List<uint>? JobList { get; set; }
	public List<IBattleChara> PlayerCharacters
	{
		get
		{
			if (JobList == null)
			{
				return Services.Objects.PlayerObjects.ToList();
			}
			else
			{
				return Services.Objects.PlayerObjects.Where(x => JobList.Contains((uint)x.ClassJob.Value.JobIndex)).ToList();
			}
		}
	}

	public OverworldTab(Plugin plugin, string tabType, List<uint>? jobList = null) : base(tabType, plugin) {
		TabType = tabType;
		JobList = jobList;
	}

	public override void Draw()
    {
        List<IBattleChara> filteredCharacters = this.Plugin.Configuration.TargetRangeLimit ? PlayerCharacters.FindAll((x) => OCStatusInfoFunctions.IsInRange(x)) : PlayerCharacters;
		ImGui.Text($"Total Characters: {filteredCharacters.Count.ToString()}");
		ImGui.SetNextItemWidth(-1 * ImGui.GetIO().FontGlobalScale);
		OverworldStatusInfoFunctions.GenerateStatusTable(filteredCharacters, this.Plugin.Configuration, TabType == "Dead" ? "Dead" : "none");
	}
}