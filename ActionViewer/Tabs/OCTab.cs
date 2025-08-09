using ActionViewer.Functions;
using ActionViewer.Windows;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;

namespace ActionViewer.Tabs;

public class OCTab : MainWindowTab
{
	public string TabType { get; set; }
	public List<uint>? JobList { get; set; }
	public List<IPlayerCharacter> PlayerCharacters
	{
		get
		{
			if (JobList == null)
			{
				return this.Plugin.ActionViewer.getPlayerCharacters();
			}
			else
			{
				return this.Plugin.ActionViewer.getPlayerCharacters().Where(x => JobList.Contains((uint)x.ClassJob.Value.JobIndex)).ToList();
			}
		}
	}

	public OCTab(Plugin plugin, string tabType, List<uint>? jobList = null) : base(tabType, plugin)
	{
		TabType = tabType;
		JobList = jobList;
	}

	public override void Draw()
	{
		List<IPlayerCharacter> filteredCharacters = this.Plugin.Configuration.TargetRangeLimit ? PlayerCharacters.FindAll((x) => OCStatusInfoFunctions.IsInRange(x)) : PlayerCharacters;
		bool inFT = Services.ClientState.LocalPlayer != null && Services.ClientState.LocalPlayer.Position.Y < -30;
		ImGui.Text($"Total Characters: {filteredCharacters.Count.ToString()}");
		ImGui.SetNextItemWidth(-1 * ImGui.GetIO().FontGlobalScale);
		OCStatusInfoFunctions.GenerateStatusTable(filteredCharacters, this.Plugin.Configuration, this.Plugin.StatusSheet, inFT, TabType == "FL" ? "FL" : TabType == "Dead Chemist" ? "Dead Chemist" : "none");
	}
}