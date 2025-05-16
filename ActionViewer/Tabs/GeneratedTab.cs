using ActionViewer.Functions;
using ActionViewer.Windows;
using Dalamud.Game.ClientState.Objects.SubKinds;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;

namespace ActionViewer.Tabs;

public class GeneratedTab : MainWindowTab
{
	private string searchText = string.Empty;

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

	public GeneratedTab(Plugin plugin, string tabType, List<uint>? jobList = null) : base(tabType, plugin) {
		TabType = tabType;
		JobList = jobList;
	}

	public override void Draw()
	{
		ImGui.SetNextItemWidth(-1 * ImGui.GetIO().FontGlobalScale);
		ImGui.InputText("", ref searchText, 256);
		StatusInfoFunctions.GenerateStatusTable(PlayerCharacters, searchText, this.Plugin.Configuration, this.Plugin.BozjaCache, this.Plugin.EurekaAction, this.Plugin.ItemSheet, TabType == "No Ess." ? "noEss" : "none");
	}
}