// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace WinDMAManager.List
{
    public class UnallowedDMAList
    {
        public int N { get; set; }
        public string Description { get; set; }
        public string DeviceID { get; set; }

        public UnallowedDMAList(int num, string desc, string devid)
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
            UnallowedDMA.DMADevice.Add(new UnallowedDMAList(num, desc, devId));
        }
    }

    public static class UnallowedDMA
    {
        public static List<UnallowedDMAList> DMADevice { get; set; } = new List<UnallowedDMAList>();
    }
}