using Dalamud.Game.ClientState.Objects.SubKinds;

namespace ActionViewer.Models
{
	public class OCCharRow
	{
		public IPlayerCharacter character { get; set; }
		public uint jobId { get; set; }
		public string playerName { get; set; }
		public OCStatusInfo statusInfo { get; set; }
	}
	public class OCStatusInfo
	{
		public OCStatusInfo() {
			masteryLevel = 0;
		}
		public Lumina.Excel.Sheets.Status? phantomJob { get; set; }
		public ushort jobLevel { get; set; }
		public ushort masteryLevel { get; set; }
		public uint masteryIcon
		{
			get
			{
				if (masteryLevel != 0)
				{
					return (uint)(219961 + masteryLevel - 1);
				}
				return 219321;
			}
		}
		public uint jobIcon
		{
			get
			{
				if (phantomJob != null)
				{
					return phantomJob.Value.Icon;
				}
				return 219321;
			}
		}
	}
}
