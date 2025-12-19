using ActionViewer.Models;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Interface.Textures;
using Dalamud.Bindings.ImGui;
using Lumina.Excel;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;

namespace ActionViewer.Functions
{
	public static class OCStatusInfoFunctions
	{
		public static bool IsInRange(IGameObject? target)
		{
			return target != null && target.YalmDistanceX < 50;
		}

		private static OCStatusInfo GetStatusInfo(StatusList statusList, ExcelSheet<Lumina.Excel.Sheets.Status> statusSheet)
		{
			OCStatusInfo statusInfo = new OCStatusInfo();

			foreach (Status status in statusList)
			{
				uint statusId = status.StatusId;
				if ((statusId >= 4358 && statusId <= 4369) || statusId == 4242)
				{
					statusInfo.jobLevel = (ushort)(status.Param % 256);
					statusInfo.phantomJob = statusSheet.GetRow(statusId);
				}
				if (statusId == 4226)
				{
					statusInfo.masteryLevel = status.Param;
				}
				if (statusId == 4262)
				{
					statusInfo.resStacks = status.Param;
				}
			}
			return statusInfo;
		}

		private static List<OCCharRow> GenerateRows(List<IBattleChara> playerCharacters, ExcelSheet<Lumina.Excel.Sheets.Status> statusSheet, bool targetRangeLimit)
		{
			List<OCCharRow> charRowList = new List<OCCharRow>();
			foreach (IBattleChara character in playerCharacters)
			{
				// get player name, job ID, status list
				OCCharRow row = new OCCharRow();
				row.character = character;
				row.playerName = character.Name.ToString();
				row.jobId = (uint)character.ClassJob.RowId;
				row.statusInfo = GetStatusInfo(character.StatusList, statusSheet);
				charRowList.Add(row);
			}
			return charRowList;
		}

		public static void GenerateStatusTable(List<IBattleChara> playerCharacters, Configuration configuration, ExcelSheet<Lumina.Excel.Sheets.Status> statusSheet, bool inFT, string filter = "none")
		{
			ImGuiTableFlags tableFlags = ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.Sortable | ImGuiTableFlags.ScrollY;// | ImGuiTableFlags.SizingFixedFit;
			var iconSize = ImGui.GetTextLineHeight() * 2f;
			var iconSizeVec = new Vector2(iconSize, iconSize);
			int columnCount = inFT ? 6 : 5;


			List<OCCharRow> charRowList = GenerateRows(playerCharacters, statusSheet, configuration.TargetRangeLimit);

			if (ImGui.BeginTable("table1", configuration.AnonymousMode ? columnCount - 1 : columnCount, tableFlags))
			{
				ImGui.TableSetupScrollFreeze(1, 1);
				ImGui.TableSetupColumn("Job", ImGuiTableColumnFlags.WidthFixed, 34f, (int)charColumns.Job);
				ImGui.TableSetupColumn("PJ", ImGuiTableColumnFlags.WidthFixed, 28f, (int)charColumns.PJ);
				ImGui.TableSetupColumn("Lv", ImGuiTableColumnFlags.WidthFixed, 28f, (int)charColumns.Lv);
				ImGui.TableSetupColumn("PM", ImGuiTableColumnFlags.WidthFixed, 28f, (int)charColumns.PM);
				if (inFT)
					ImGui.TableSetupColumn("RR", ImGuiTableColumnFlags.WidthFixed, 28f, (int)charColumns.RR);
				if (!configuration.AnonymousMode)
				{
					ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch | ImGuiTableColumnFlags.PreferSortDescending, 1f, (int)charColumns.Name);
				}
				ImGui.TableHeadersRow();
				ImGuiTableSortSpecsPtr sortSpecs = ImGui.TableGetSortSpecs();
				charRowList = SortCharDataWithSortSpecs(sortSpecs, charRowList);
				var clipper = new ImGuiListClipper();
				clipper.Begin(charRowList.Count, 0);

				while (clipper.Step())
				{
					for (var i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
					{
						var row = charRowList[i];
						if (filter == "none" ||
						(filter == "FL" && row.statusInfo.phantomJob.Value.RowId == 4242) ||
						(filter == "Dead Chemist" && row.statusInfo.phantomJob.Value.RowId == 4367 && row.character.IsDead)
						)
						{
							// player job, name
							ImGui.TableNextColumn();

							ImGui.Image(
								Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(row.jobIconId)).GetWrapOrEmpty().Handle,
								iconSizeVec, Vector2.Zero, Vector2.One);
							var hover = ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled);
							var left = hover && ImGui.IsMouseClicked(ImGuiMouseButton.Left);
							if (left)
							{
								Plugin.TargetManager.Target = row.character;
							}
							ImGui.TableNextColumn();
							ImGui.Image(
								Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(row.statusInfo.jobIcon)).GetWrapOrEmpty().Handle,
								new Vector2(iconSize * (float)0.8, iconSize));
							ImGui.TableNextColumn();
							ImGui.Text(row.statusInfo.jobLevel.ToString());
							ImGui.TableNextColumn();
							ImGui.Image(
								Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(row.statusInfo.masteryIcon)).GetWrapOrEmpty().Handle,
								new Vector2(iconSize * (float)0.8, iconSize));
							if (inFT)
							{
								ImGui.TableNextColumn();
								ImGui.Image(
									Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(row.statusInfo.resIcon)).GetWrapOrEmpty().Handle,
									new Vector2(iconSize * (float)0.8, iconSize));
							}
							if (!configuration.AnonymousMode)
							{
								ImGui.TableNextColumn();
								ImGui.Selectable(row.playerName, false);
								hover = ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled);
								left = hover && ImGui.IsMouseClicked(ImGuiMouseButton.Left);
								if (left)
								{
									Plugin.TargetManager.Target = row.character;
								}
							}
						}
					}
				}
				ImGui.EndTable();
			}
		}
		public enum charColumns
		{
			Job,
			PJ,
			Lv,
			PM,
			RR,
			Name
		}

		public static List<OCCharRow> SortCharDataWithSortSpecs(ImGuiTableSortSpecsPtr sortSpecs, List<OCCharRow> charDataList)
		{

			IEnumerable<OCCharRow> sortedCharaData = charDataList;

			for (int i = 0; i < sortSpecs.SpecsCount; i++)
			{
				ImGuiTableColumnSortSpecsPtr columnSortSpec = sortSpecs.Specs;

				switch ((charColumns)columnSortSpec.ColumnUserID)
				{
					case charColumns.Job:
						if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
						{
							sortedCharaData = sortedCharaData.OrderBy(o => o.jobSort);
						}
						else
						{
							sortedCharaData = sortedCharaData.OrderByDescending(o => o.jobSort);
						}
						break;
					case charColumns.Name:
						if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
						{
							sortedCharaData = sortedCharaData.OrderBy(o => o.playerName);
						}
						else
						{
							sortedCharaData = sortedCharaData.OrderByDescending(o => o.playerName);
						}
						break;
					case charColumns.PJ:
						if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
						{
							sortedCharaData = sortedCharaData.OrderBy(o => o.statusInfo.jobIcon);
						}
						else
						{
							sortedCharaData = sortedCharaData.OrderByDescending(o => o.statusInfo.jobIcon);
						}
						break;
					case charColumns.Lv:
						if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
						{
							sortedCharaData = sortedCharaData.OrderBy(o => o.statusInfo.jobLevel);
						}
						else
						{
							sortedCharaData = sortedCharaData.OrderByDescending(o => o.statusInfo.jobLevel);
						}
						break;
					case charColumns.PM:
						if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
						{
							sortedCharaData = sortedCharaData.OrderBy(o => o.statusInfo.masteryLevel);
						}
						else
						{
							sortedCharaData = sortedCharaData.OrderByDescending(o => o.statusInfo.masteryLevel);
						}
						break;
					case charColumns.RR:
						if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
						{
							sortedCharaData = sortedCharaData.OrderBy(o => o.statusInfo.resStacks);
						}
						else
						{
							sortedCharaData = sortedCharaData.OrderByDescending(o => o.statusInfo.resStacks);
						}
						break;
					default:
						break;
				}
			}

			return sortedCharaData.ToList();
		}
	}
}
