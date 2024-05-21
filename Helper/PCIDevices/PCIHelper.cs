using WinDMAManager.List;
using System.Management;

namespace WinDMAManager.Helper.PCIDevices;

public static class PCIHelper
{
    // https://stackoverflow.com/questions/11966684/how-to-get-the-device-id-of-pci-devices
    public static void SearchAllPCIDevice()
    {
        ManagementObjectSearcher pciSearcher =
            new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PnPEntity");

        var N = 0;
        
        foreach (var o in pciSearcher.Get())
        {
            var obj = (ManagementObject)o;
            if (obj["DeviceID"].ToString()!.Substring(0,3) != "PCI") continue;
            var desc = obj["Description"].ToString()!;
            var devId = obj["DeviceID"].ToString()!.Substring(0,21);
            
            //Console.WriteLine($"Description: {obj["Description"]} - DeviceID: {obj["DeviceID"]}");
            PCIDeviceList.AddItem(N, desc, devId);
            N++;
        }
    }

    public static void MatchPCIID()
    {
        List<AllowedDMAList> AllowedList = AllowedDMA.DMADevice;
        List<UnallowedDMAList> UnallowedList = UnallowedDMA.DMADevice;
        List<PCIDeviceList> PCIDevList = PCIDevice.PciDevice;

        foreach (var pciList in PCIDevList)
        {
            if (pciList.Match == null)
            {
                if (AllowedList.Any(allowedItem => allowedItem.DeviceID == pciList.DeviceID))
                {
                    pciList.Match = "Y";
                }
                else if (UnallowedList.Any(unallowedItem => unallowedItem.DeviceID == pciList.DeviceID))
                {
                    pciList.Match = "N";
                }
                else
                {
                    pciList.Match = "?";
                }
            }
        }
    }
}