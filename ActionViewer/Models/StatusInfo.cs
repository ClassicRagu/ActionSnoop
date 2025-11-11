namespace ActionViewer.Models
{
    public class CharRow : BaseCharRow
    {
        public StatusInfo statusInfo { get; set; }
    }
    public class StatusInfo
    {
        public Lumina.Excel.Sheets.Action? leftLuminaStatusInfo { get; set; }
        public Lumina.Excel.Sheets.Action? rightLuminaStatusInfo { get; set; }
        public Lumina.Excel.Sheets.Item? itemLuminaInfo { get; set; }
		public int essenceId { get; set; }
        public int reraiserStatus { get; set; }
        public string essenceName
        {
            get
            {
                if(itemLuminaInfo != null)
                {
                    return itemLuminaInfo.Value.Name.ExtractText();
                }
                return null;
            }
        }
        public uint essenceIconID
        {
            get
            {
                if (itemLuminaInfo != null)
                {
                    return itemLuminaInfo.Value.Icon;
                }
                return 26;
            }

        }
        public uint rightIconID
        {
            get
            {
                if (rightLuminaStatusInfo != null)
                {
				    return rightLuminaStatusInfo.Value.Icon;
				}
                return 33;
            }
        }
        public uint leftIconID
        {
            get
            {
                if (leftLuminaStatusInfo != null)
                {
                    return leftLuminaStatusInfo.Value.Icon;
				}
                return 33;
            }
        }
        public uint reraiserIconID
        {
            get
            {
                switch(reraiserStatus)
                {
                    case 1:
                        return 219301;
                    case 2:
                        return 219302;
                    default:
                        return 219321;
                }
            }
        }
    }
}
