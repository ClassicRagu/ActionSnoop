using ActionViewer.Functions;
using ActionViewer.Windows;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Types;

namespace ActionViewer.Tabs;

public class STQEurekaTab : MainWindowTab
{
	private string searchText = string.Empty;

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

	public STQEurekaTab(Plugin plugin, string tabType, List<uint>? jobList = null) : base(tabType, plugin) {
		TabType = tabType;
		JobList = jobList;
	}

	public override void Draw()
	{
		ImGui.SetNextItemWidth(-1 * ImGui.GetIO().FontGlobalScale);
		ImGui.InputText("", ref searchText, 256);
		STQEurekaStatusInfoFunctions.GenerateStatusTable(PlayerCharacters, searchText, this.Plugin.Configuration, this.Plugin.BozjaCache, this.Plugin.EurekaAction, this.Plugin.ItemSheet, TabType == "No Ess." ? "noEss" : "none");
	}
}