using ActionViewer.Tabs;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using Dalamud.Bindings.ImGui;
using System.Collections.Generic;
using System.Numerics;
using ActionViewer.Functions;

namespace ActionViewer.Windows;

public class MainWindow : Window
{
	private Plugin plugin;
	private List<MainWindowTab> tabs;
	/*
     * 920 - BSF
     * 936 - DRN
     * 937 - DRS
     * 975 - Zadnor
     */
	private List<ushort> territoryTypes = new List<ushort>() { 920, 936, 937, 975, 795, 827 };
	private static List<ushort> eurekaTerritories = new List<ushort>() { 795, 827 };
	private List<uint> tanks = new List<uint>() { 1, 3, 12, 17 };
	private List<uint> healers = new List<uint>() { 6, 9, 13, 20 };
	private List<uint> casterDPS = new List<uint>() { 7, 8, 15, 22 };
	private List<uint> melee = new List<uint>() { 2, 4, 10, 14, 19, 21 };
	private List<uint> physRanged = new List<uint>() { 5, 11, 18 };
	private List<uint> xivClasses = new List<uint>() { 0 };

	public MainWindow(Plugin plugin) : base("ActionViewer")
	{
		SizeConstraints = new WindowSizeConstraints
		{
			MinimumSize = new Vector2(300, 300) * ImGuiHelpers.GlobalScale,
			MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
		};
		Size = new Vector2(310, 200);
		SizeCondition = ImGuiCond.FirstUseEver;

		this.plugin = plugin;
		CreateTabs();
	}

	public void CreateTabs()
	{
		if (eurekaTerritories.Contains(Services.ClientState.TerritoryType))
		{
			this.tabs = new List<MainWindowTab> {
			new STQEurekaTab(this.plugin, "Main"),
			new STQEurekaTab(this.plugin, "Tanks", tanks),
			new STQEurekaTab(this.plugin, "Healers", healers),
			new STQEurekaTab(this.plugin, "Melee", melee),
			new STQEurekaTab(this.plugin, "Phys Ranged", physRanged),
			new STQEurekaTab(this.plugin, "Caster", casterDPS),
			new STQEurekaTab(this.plugin, "Classes", xivClasses),
			};
		} else if (territoryTypes.Contains(Services.ClientState.TerritoryType))  {
			this.tabs = new List<MainWindowTab> {
			new STQEurekaTab(this.plugin, "Main"),
			new STQEurekaTab(this.plugin, "No Ess."),
			new STQEurekaTab(this.plugin, "Tanks", tanks),
			new STQEurekaTab(this.plugin, "Healers", healers),
			new STQEurekaTab(this.plugin, "Melee", melee),
			new STQEurekaTab(this.plugin, "Phys Ranged", physRanged),
			new STQEurekaTab(this.plugin, "Caster", casterDPS),
			new STQEurekaTab(this.plugin, "Classes", xivClasses),
			};
		} else if (Services.ClientState.TerritoryType == 1252)
		{
			this.tabs = new List<MainWindowTab>
			{
				new OCTab(this.plugin, "Main"),
				new OCTab(this.plugin, "FL"),
				new OCTab(this.plugin, "Dead Chemist"),
				new OCTab(this.plugin, "Tanks", tanks),
				new OCTab(this.plugin, "Healers", healers),
				new OCTab(this.plugin, "Melee", melee),
				new OCTab(this.plugin, "Phys Ranged", physRanged),
				new OCTab(this.plugin, "Caster", casterDPS),
				new OCTab(this.plugin, "Classes", xivClasses)
			};
		} else
        {
            this.tabs = new List<MainWindowTab>
			{
				new OverworldTab(this.plugin, "Main"),
				new OverworldTab(this.plugin, "Dead"),
				new OverworldTab(this.plugin, "Tanks", tanks),
				new OverworldTab(this.plugin, "Healers", healers),
				new OverworldTab(this.plugin, "Melee", melee),
				new OverworldTab(this.plugin, "Phys Ranged", physRanged),
				new OverworldTab(this.plugin, "Caster", casterDPS),
				new OverworldTab(this.plugin, "Classes", xivClasses)
			};
        }
	}

	public void Reset()
	{
		this.tabs.Clear();
		CreateTabs();
	}

	public void Dispose()
	{
		this.tabs.Clear();
	}

	public override void Draw()
	{
		if (this.plugin.Configuration.UnrestrictZones || territoryTypes.Contains(Services.ClientState.TerritoryType))
		{
			if (ImGui.BeginTabBar("##ActionViewer_MainWindowTabs", ImGuiTabBarFlags.None))
			{
				foreach (var tab in tabs)
				{
					if (ImGui.BeginTabItem(tab.Name))
					{
						tab.Draw();
						ImGui.EndTabItem();
					}
				}

				ImGui.EndTabBar();
			}
		}
		else
		{
			ImGui.Text("Please Enter a Field Operation Zone");
		}
	}
}