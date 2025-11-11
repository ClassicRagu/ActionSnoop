namespace ActionViewer.Models
{
	public class OCCharRow : BaseCharRow
	{
		public OCStatusInfo statusInfo { get; set; }
	}
	public class OCStatusInfo
	{
		public OCStatusInfo() {
			masteryLevel = 0;
			resStacks = 0;
		}
		public Lumina.Excel.Sheets.Status? phantomJob { get; set; }
		public ushort jobLevel { get; set; }
		public ushort masteryLevel { get; set; }
		public ushort resStacks { get; set; }
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
		public uint resIcon
		{
			get
			{
				if (resStacks != 0)
				{
					return (uint)(218286 + resStacks - 1);
				}
				return 219321;
			}
		}
	}
}
