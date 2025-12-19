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
	public static class STQEurekaStatusInfoFunctions
	{
		public static bool IsInRange(IGameObject? target)
		{
			return target != null && target.YalmDistanceX < 50;
		}

		private static List<ushort> eurekaTerritories = new List<ushort>() { 795, 827 };
		private static List<ushort> delubrumTerritories = new List<ushort>() { 936, 937 };
		private static List<int> essenceIds = new List<int>() { 2311, 2312, 2313, 2314, 2315, 2316, 2317, 2318, 2319, 2320, 2321, 2322, 2323, 2324, 2325, 2434, 2435, 2436, 2437, 2438, 2439, };
		private static STQEurekaStatusInfo GetStatusInfo(StatusList statusList, ExcelSheet<Lumina.Excel.Sheets.MYCTemporaryItem> bozjaCache, ExcelSheet<Lumina.Excel.Sheets.EurekaMagiaAction> eurekaAction, ExcelSheet<Lumina.Excel.Sheets.Item> itemSheet)
		{
			STQEurekaStatusInfo statusInfo = new STQEurekaStatusInfo();

			foreach (IStatus status in statusList)
			{
				int statusId = (int)status.StatusId;
				if (essenceIds.Contains(statusId))
				{
					statusInfo.essenceId = statusId;
					uint essence = (uint)(statusId > 2325 ? 32168 + statusId - 2434 : 30940 + statusId - 2311);
					statusInfo.itemLuminaInfo = itemSheet.GetRow(essence);
				}
				if (statusId.Equals(2348))
				{
					uint leftId = (uint)status.Param % 256;
					uint rightId = (status.Param - leftId) / 256;

					if (leftId > 0)
						statusInfo.leftLuminaStatusInfo = bozjaCache.GetRow(leftId).Action.Value;

					if (rightId > 0)
						statusInfo.rightLuminaStatusInfo = bozjaCache.GetRow(rightId).Action.Value;
				}
				if (statusId.Equals(1618))
				{
					uint leftId = (uint)status.Param % 256;
					uint rightId = (status.Param - leftId) / 256;

					if (leftId > 0)
						statusInfo.leftLuminaStatusInfo = eurekaAction.GetRow(leftId).Action.Value;

					if (rightId > 0)
						statusInfo.rightLuminaStatusInfo = eurekaAction.GetRow(rightId).Action.Value;
				}
				if (statusId.Equals(2355))
				{
					statusInfo.reraiserStatus = status.Param == 70 ? 1 : 2;
				}
				if (statusId.Equals(1641))
				{
					statusInfo.reraiserStatus = 1;
				}
			}
			return statusInfo;
		}

		private static List<STQEurekaCharRow> GenerateRows(List<IBattleChara> playerCharacters, ExcelSheet<Lumina.Excel.Sheets.MYCTemporaryItem> bozjaCache, ExcelSheet<Lumina.Excel.Sheets.EurekaMagiaAction> eurekaAction, ExcelSheet<Lumina.Excel.Sheets.Item> itemSheet, bool targetRangeLimit)
		{
			List<STQEurekaCharRow> charRowList = new List<STQEurekaCharRow>();
			foreach (IBattleChara character in playerCharacters)
			{
				if (!targetRangeLimit || IsInRange(character))
				{
					// get player name, job ID, status list
					STQEurekaCharRow row = new STQEurekaCharRow();
					row.character = character;
					row.playerName = character.Name.ToString();
					row.jobId = (uint)character.ClassJob.RowId;
					row.statusInfo = GetStatusInfo(character.StatusList, bozjaCache, eurekaAction, itemSheet);
					charRowList.Add(row);
				}
			}
			return charRowList;
		}

		public static void GenerateStatusTable(List<IBattleChara> playerCharacters, string searchText, Configuration configuration, ExcelSheet<Lumina.Excel.Sheets.MYCTemporaryItem> bozjaCache, ExcelSheet<Lumina.Excel.Sheets.EurekaMagiaAction> eurekaAction, ExcelSheet<Lumina.Excel.Sheets.Item> itemSheet, string filter = "none")
		{
			ImGuiTableFlags tableFlags = ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.Sortable | ImGuiTableFlags.ScrollY;// | ImGuiTableFlags.SizingFixedFit;
			var iconSize = ImGui.GetTextLineHeight() * 2f;
			var iconSizeVec = new Vector2(iconSize, iconSize);
			bool eurekaTerritory = eurekaTerritories.Contains(Services.ClientState.TerritoryType);
			int columnCount = eurekaTerritory ? 5 : 6;
			bool delubrumTerritory = delubrumTerritories.Contains(Services.ClientState.TerritoryType);


			List<STQEurekaCharRow> charRowList = GenerateRows(playerCharacters, bozjaCache, eurekaAction, itemSheet, configuration.TargetRangeLimit);

			if (ImGui.BeginTable("table1", configuration.AnonymousMode ? columnCount - 1 : columnCount, tableFlags))
			{
				ImGui.TableSetupScrollFreeze(1, 1);
				ImGui.TableSetupColumn("Job", ImGuiTableColumnFlags.WidthFixed, 34f, (int)charColumns.Job);
				if (!configuration.AnonymousMode)
				{
					ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch | ImGuiTableColumnFlags.PreferSortDescending, 1f, (int)charColumns.Name);
				}
				if (!eurekaTerritory)
				{
					ImGui.TableSetupColumn("RR", ImGuiTableColumnFlags.WidthFixed, 28f, (int)charColumns.Reraiser);
					ImGui.TableSetupColumn("Ess.", ImGuiTableColumnFlags.WidthFixed, 34f, (int)charColumns.Essence);
				}
				else
				{
					ImGui.TableSetupColumn("Remembered", ImGuiTableColumnFlags.WidthFixed, 28f, (int)charColumns.Reraiser);
				}
				ImGui.TableSetupColumn("Left", ImGuiTableColumnFlags.WidthFixed, 34f, (int)charColumns.Left);
				ImGui.TableSetupColumn("Right", ImGuiTableColumnFlags.WidthFixed, 34f, (int)charColumns.Right);
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
						if ((searchText == string.Empty ||
							(row.statusInfo.rightIconID != 33 && (row.statusInfo.rightLuminaStatusInfo.Value.Name.ExtractText().ToLowerInvariant().IndexOf(searchText.ToLowerInvariant()) != -1)) ||
							(row.statusInfo.leftIconID != 33 && (row.statusInfo.leftLuminaStatusInfo.Value.Name.ExtractText().ToLowerInvariant().IndexOf(searchText.ToLowerInvariant()) != -1))) &&
						(filter == "none" ||
						// in DR we want to also filter out non-pure essences. Gambler is luckily the first pure essence by ID so it's easy to filter
						(filter == "noEss" && (row.statusInfo.essenceIconID == 26 || (delubrumTerritory && row.statusInfo.essenceId < 2435))))
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

							// reraiser
							ImGui.TableNextColumn();
							ImGui.Image(
								Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(row.statusInfo.reraiserIconID)).GetWrapOrEmpty().Handle,
								new Vector2(iconSize * (float)0.8, iconSize));

							if (!eurekaTerritory)
							{
								// essence
								ImGui.TableNextColumn();
								ImGui.Image(
									Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(row.statusInfo.essenceIconID)).GetWrapOrEmpty().Handle,
									iconSizeVec, Vector2.Zero, Vector2.One);
								if (configuration.Tooltips && ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) && row.statusInfo.essenceName != null)
								{
									ImGui.SetTooltip(row.statusInfo.essenceName);
								}
							}

							// left/right actions
							ImGui.TableNextColumn();
							ImGui.Image(
								Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(row.statusInfo.leftIconID)).GetWrapOrEmpty().Handle,
								iconSizeVec, Vector2.Zero, Vector2.One);
							if (configuration.Tooltips && ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) && row.statusInfo.leftLuminaStatusInfo != null)
							{
								ImGui.SetTooltip(row.statusInfo.leftLuminaStatusInfo.Value.Name.ExtractText());

							}
							ImGui.TableNextColumn();
							ImGui.Image(
								Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(row.statusInfo.rightIconID)).GetWrapOrEmpty().Handle,
								iconSizeVec, Vector2.Zero, Vector2.One);
							if (configuration.Tooltips && ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) && row.statusInfo.rightLuminaStatusInfo != null)
							{
								ImGui.SetTooltip(row.statusInfo.rightLuminaStatusInfo.Value.Name.ExtractText());

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
			Name,
			Reraiser,
			Essence,
			Left,
			Right
		}

		public static List<STQEurekaCharRow> SortCharDataWithSortSpecs(ImGuiTableSortSpecsPtr sortSpecs, List<STQEurekaCharRow> charDataList)
		{
			IEnumerable<STQEurekaCharRow> sortedCharaData = charDataList;

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
					case charColumns.Essence:
						if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
						{
							sortedCharaData = sortedCharaData.OrderBy(o => o.statusInfo.essenceIconID);
						}
						else
						{
							sortedCharaData = sortedCharaData.OrderByDescending(o => o.statusInfo.essenceIconID);
						}
						break;
					case charColumns.Reraiser:
						if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
						{
							sortedCharaData = sortedCharaData.OrderBy(o => o.statusInfo.reraiserStatus);
						}
						else
						{
							sortedCharaData = sortedCharaData.OrderByDescending(o => o.statusInfo.reraiserStatus);
						}
						break;
					case charColumns.Left:
						if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
						{
							sortedCharaData = sortedCharaData.OrderBy(o => o.statusInfo.leftIconID);
						}
						else
						{
							sortedCharaData = sortedCharaData.OrderByDescending(o => o.statusInfo.leftIconID);
						}
						break;
					case charColumns.Right:
						if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
						{
							sortedCharaData = sortedCharaData.OrderBy(o => o.statusInfo.rightIconID);
						}
						else
						{
							sortedCharaData = sortedCharaData.OrderByDescending(o => o.statusInfo.rightIconID);
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
