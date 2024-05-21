// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace WinDMAManager.List
{
    public class PCIDeviceList
    {
        public int N { get; set; }
        public string Description { get; set; }
        public string DeviceID { get; set; }
        
        public string? Match { get; set; } 

        public PCIDeviceList(int num, string desc, string devid)
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
            PCIDevice.PciDevice.Add(new PCIDeviceList(num, desc, devId));
        }
    }

    public static class PCIDevice
    {
        public static List<PCIDeviceList> PciDevice { get; set; } = new List<PCIDeviceList>();
    }
}