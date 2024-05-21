namespace WinDMAManager.List
{
    public class AllowedDMAList
    {
        public int N { get; set; }
        public string Description { get; set; }
        public string DeviceID { get; set; }

        public AllowedDMAList(int num, string desc, string devid)
        {
            N = num;
            Description = desc;
            DeviceID = devid;
        }

        public override string ToString()
        {
            return $"{Description} {DeviceID}";
        }

        public static void AddItem(int num, string desc, string devId)
        {
            AllowedDMA.DMADevice.Add(new AllowedDMAList(num, desc, devId));
        }
    }

    public static class AllowedDMA
    {
        public static List<AllowedDMAList> DMADevice { get; set; } = new List<AllowedDMAList>();
    }
}