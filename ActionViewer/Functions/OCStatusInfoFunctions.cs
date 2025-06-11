using ActionViewer.Models;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Statuses;
using Dalamud.Interface.Textures;
using ImGuiNET;
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

		private static List<OCCharRow> GenerateRows(List<IPlayerCharacter> playerCharacters, ExcelSheet<Lumina.Excel.Sheets.Status> statusSheet, bool targetRangeLimit)
		{
			List<OCCharRow> charRowList = new List<OCCharRow>();
			foreach (IPlayerCharacter character in playerCharacters)
			{
				// get player name, job ID, status list
				OCCharRow row = new OCCharRow();
				row.character = character;
				row.playerName = character.Name.ToString();
				row.jobId = (uint)character.ClassJob.Value.JobIndex;
				row.statusInfo = GetStatusInfo(character.StatusList, statusSheet);
				charRowList.Add(row);
			}
			return charRowList;
		}

		public static void GenerateStatusTable(List<IPlayerCharacter> playerCharacters, Configuration configuration, ExcelSheet<Lumina.Excel.Sheets.Status> statusSheet, bool inFT, string filter = "none")
		{
			ImGuiTableFlags tableFlags = ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.Sortable;// | ImGuiTableFlags.SizingFixedFit;
			var iconSize = ImGui.GetTextLineHeight() * 2f;
			var iconSizeVec = new Vector2(iconSize, iconSize);
			int columnCount = inFT ? 6 : 5;


			List<OCCharRow> charRowList = GenerateRows(playerCharacters, statusSheet, configuration.TargetRangeLimit);

			if (ImGui.BeginTable("table1", configuration.AnonymousMode ? columnCount - 1 : columnCount, tableFlags))
			{
				ImGui.TableSetupColumn("Job", ImGuiTableColumnFlags.WidthFixed, 34f, (int)charColumns.Job);
				ImGui.TableSetupColumn("PJ", ImGuiTableColumnFlags.WidthFixed, 28f, (int)charColumns.PJ);
				ImGui.TableSetupColumn("Lv", ImGuiTableColumnFlags.WidthFixed, 28f, (int)charColumns.Lv);
				ImGui.TableSetupColumn("PM", ImGuiTableColumnFlags.WidthFixed, 28f, (int)charColumns.PM);
				if(inFT)
					ImGui.TableSetupColumn("RR", ImGuiTableColumnFlags.WidthFixed, 28f, (int)charColumns.RR);
				if (!configuration.AnonymousMode)
				{
					ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch | ImGuiTableColumnFlags.PreferSortDescending, 1f, (int)charColumns.Name);
				}
				ImGui.TableHeadersRow();
				ImGuiTableSortSpecsPtr sortSpecs = ImGui.TableGetSortSpecs();
				charRowList = SortCharDataWithSortSpecs(sortSpecs, charRowList);

				foreach (OCCharRow row in charRowList)
				{

					if (filter == "none" ||
						(filter == "FL" && row.statusInfo.phantomJob.Value.RowId == 4242) ||
						(filter == "Dead Chemist" && row.statusInfo.phantomJob.Value.RowId == 4367 && row.character.IsDead)
						)
					{
						// player job, name
						ImGui.TableNextColumn();

						uint jobIconId = 62118;
						if (row.jobId >= 10)
						{
							jobIconId += 2 + row.jobId;
						}
						else if (row.jobId >= 8)
						{
							jobIconId += 1 + row.jobId;
						}
						else
						{
							jobIconId += row.jobId;
						}

						ImGui.Image(
							Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(jobIconId)).GetWrapOrEmpty().ImGuiHandle,
							iconSizeVec, Vector2.Zero, Vector2.One);
						var hover = ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled);
						var left = hover && ImGui.IsMouseClicked(ImGuiMouseButton.Left);
						if (left)
						{
							Plugin.TargetManager.Target = row.character;
						}
						ImGui.TableNextColumn();
						ImGui.Image(
							Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(row.statusInfo.jobIcon)).GetWrapOrEmpty().ImGuiHandle,
							new Vector2(iconSize * (float)0.8, iconSize));
						ImGui.TableNextColumn();
						ImGui.Text(row.statusInfo.jobLevel.ToString());
						ImGui.TableNextColumn();
						ImGui.Image(
							Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(row.statusInfo.masteryIcon)).GetWrapOrEmpty().ImGuiHandle,
							new Vector2(iconSize * (float)0.8, iconSize));
						if (inFT) {
							ImGui.TableNextColumn();
							ImGui.Image(
								Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(row.statusInfo.resIcon)).GetWrapOrEmpty().ImGuiHandle,
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

			Dictionary<uint, uint> jobSort = new Dictionary<uint, uint>()
			{
				{1, 1 }, // PLD
                {3, 2 }, // WAR
                {12, 3}, // DRK
                {17, 4 }, // GNB
                {6 , 5 }, // WHM
                {9 , 6 }, // SCH
                {13, 7}, // AST
                {20 , 8 }, // SGE
                {2, 9 }, // MNK
                {4 , 10 }, // DRG
                {10 , 11 }, // NIN
                {14 , 12 }, // SAM
                {19 , 13 }, // RPR
				{21, 20 }, // VPR
				{5 , 14 }, // BRD
                {11 , 15 }, // MCH
                {18 , 16 }, // DNC
                {7 , 17 }, // BLM
                {8 , 18 }, // SMN
                {15 , 19 }, // RDM
				{22, 21 } // PCT
            };

			IEnumerable<OCCharRow> sortedCharaData = charDataList;

			for (int i = 0; i < sortSpecs.SpecsCount; i++)
			{
				ImGuiTableColumnSortSpecsPtr columnSortSpec = sortSpecs.Specs;

				switch ((charColumns)columnSortSpec.ColumnUserID)
				{
					case charColumns.Job:
						if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
						{
							sortedCharaData = sortedCharaData.OrderBy(o => jobSort.GetValueOrDefault(o.jobId));
						}
						else
						{
							sortedCharaData = sortedCharaData.OrderByDescending(o => jobSort.GetValueOrDefault(o.jobId));
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
