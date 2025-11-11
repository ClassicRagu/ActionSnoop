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
    public static class OverworldStatusInfoFunctions
    {
        public static bool IsInRange(IGameObject? target)
        {
            return target != null && target.YalmDistanceX < 50;
        }

        private static List<BaseCharRow> GenerateRows(List<IPlayerCharacter> playerCharacters)
        {
            List<BaseCharRow> charRowList = new List<BaseCharRow>();
            foreach (IPlayerCharacter character in playerCharacters)
            {
                // get player name, job ID, status list
                BaseCharRow row = new BaseCharRow();
                row.character = character;
                row.playerName = character.Name.ToString();
                row.jobId = (uint)character.ClassJob.RowId; //(uint)character.ClassJob.Value.JobIndex;
                charRowList.Add(row);
            }
            return charRowList;
        }

        public static void GenerateStatusTable(List<IPlayerCharacter> playerCharacters, Configuration configuration, string filter = "none")
        {
            ImGuiTableFlags tableFlags = ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.Sortable;// | ImGuiTableFlags.SizingFixedFit;
            var iconSize = ImGui.GetTextLineHeight() * 2f;
            var iconSizeVec = new Vector2(iconSize, iconSize);
            int columnCount = 2;


            List<BaseCharRow> charRowList = GenerateRows(playerCharacters);

            if (ImGui.BeginTable("table1", configuration.AnonymousMode ? columnCount - 1 : columnCount, tableFlags))
            {
                ImGui.TableSetupColumn("Job", ImGuiTableColumnFlags.WidthFixed, 34f, (int)charColumns.Job);
                ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch | ImGuiTableColumnFlags.PreferSortDescending, 1f, (int)charColumns.Name);
                ImGui.TableHeadersRow();
                ImGuiTableSortSpecsPtr sortSpecs = ImGui.TableGetSortSpecs();
                charRowList = SortCharDataWithSortSpecs(sortSpecs, charRowList);

                foreach (BaseCharRow row in charRowList)
                {

                    if (filter == "none" ||
                        (filter == "Dead" && row.character.IsDead)
                        )
                    {
                        // player job, name
                        ImGui.TableNextColumn();

                        uint jobIconId = 62100;
                        jobIconId += row.jobId;

                        ImGui.Image(
                            Plugin.TextureProvider.GetFromGameIcon(new GameIconLookup(jobIconId)).GetWrapOrEmpty().Handle,
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
                    }
                }
                ImGui.EndTable();
            }
        }
        public enum charColumns
        {
            Job,
            Name
        }

        public static List<BaseCharRow> SortCharDataWithSortSpecs(ImGuiTableSortSpecsPtr sortSpecs, List<BaseCharRow> charDataList)
        {

            Dictionary<uint, uint> jobSort = new Dictionary<uint, uint>()
            {
                {0, 0 }, // Default
                {8, 0}, // ARC 
                {11, 0}, // ROG 
                {1, 1 }, // PLD
                {3, 2 }, // WAR
                {14, 3}, // DRK
                {19, 4 }, // GNB
                {6 , 5 }, // WHM
                {10 , 6 }, // SCH
                {15, 7}, // AST
                {22 , 8 }, // SGE
                {2, 9 }, // MNK
                {4 , 10 }, // DRG
                {12 , 11 }, // NIN
                {16 , 12 }, // SAM
                {21 , 13 }, // RPR
				{23, 20 }, // VPR
				{5 , 14 }, // BRD
                {13 , 15 }, // MCH
                {20 , 16 }, // DNC
                {7 , 17 }, // BLM
                {9 , 18 }, // SMN
                {17 , 19 }, // RDM
				{24, 21 }, // PCT
                {18, 22 } // BLU
            };

            IEnumerable<BaseCharRow> sortedCharaData = charDataList;

            for (int i = 0; i < sortSpecs.SpecsCount; i++)
            {
                ImGuiTableColumnSortSpecsPtr columnSortSpec = sortSpecs.Specs;

                switch ((charColumns)columnSortSpec.ColumnUserID)
                {
                    case charColumns.Job:
                        if (columnSortSpec.SortDirection == ImGuiSortDirection.Ascending)
                        {
                            sortedCharaData = sortedCharaData.OrderBy(o => jobSort.GetValueOrDefault(o.jobId < 19 ? 0 : o.jobId - 18));
                        }
                        else
                        {
                            sortedCharaData = sortedCharaData.OrderByDescending(o => jobSort.GetValueOrDefault(o.jobId < 19 ? 0 : o.jobId - 18));
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
                    default:
                        break;
                }
            }

            return sortedCharaData.ToList();
        }
    }
}
