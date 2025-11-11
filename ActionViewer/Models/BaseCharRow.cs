using Dalamud.Game.ClientState.Objects.SubKinds;

namespace ActionViewer.Models
{
	public class BaseCharRow
	{
		public IPlayerCharacter character { get; set; }
		public uint jobId { get; set; }
		public string playerName { get; set; }
	}
}
