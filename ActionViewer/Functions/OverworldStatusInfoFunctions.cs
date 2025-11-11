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
            IEnumerable<BaseCharRow> sortedCharaData = charDataList;

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
                    default:
                        break;
                }
            }

            return sortedCharaData.ToList();
        }
    }
}
