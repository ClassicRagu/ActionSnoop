using System.Collections.Generic;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;

namespace ActionViewer.Models
{
	public class BaseCharRow
	{
		public IBattleChara character { get; set; }
		public uint jobId { get; set; }
		public string playerName { get; set; }
		public uint jobIconId {get
            {
                return 62100 + jobId;
            }
        }
		public uint jobSort
		{
			get
			{
				Dictionary<uint, uint> jobDictionary = new Dictionary<uint, uint>()
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
				return jobId < 19 ? 0 : jobDictionary.GetValueOrDefault(jobId - 18);
			}
		}
		
	}
}
