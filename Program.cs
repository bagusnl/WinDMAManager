
using WinDMAManager.Helper;
using WinDMAManager.Helper.PCIDevices;
using WinDMAManager.List;

namespace WinDMAManager
{
    internal static class Program
    {
        public static bool? _isInitialized;
        
        private static int Main()
        {
            // Run initialization once
            _isInitialized ??= Initialize();
            if ((bool)!_isInitialized)
            {
                Console.WriteLine("Failure to initialize! Exiting app...");
                Environment.Exit(100);
            }
            
            Console.WriteLine("Match List:\r\n\tY = In allowed list\r\n\tN = In unallowed list\r\n\t? = Not found in any list");
            foreach(var l in PCIDevice.PciDevice ) Console.WriteLine($"Found Device #{l.N.ToString("D3")}, Match = {l.Match!}, {l.DeviceID}, {l.Description}");
            Console.WriteLine();

            var input = SelectOption();
            switch (input.ToUpper())
            {
                case "A":
                    // TODO: implement add all to allow
                    throw new NotImplementedException();
                    break;
                case "D":
                    // TODO: implement add all to unallow
                    throw new NotImplementedException();
                    break;
                case "X":
                    Environment.Exit(0);
                    break;
                default:
                    if (int.TryParse(input, out int val))
                    {
                        ModifyDevice(val);
                        break;
                    }
                    Console.WriteLine("Invalid input!");
                    break;
            }

            return 0;
        }

        private static bool Initialize()
        {
            try
            {
                // Initialize PCI list
                PCIHelper.SearchAllPCIDevice();
            
                // Takeover DMA registry
                RegistryHelper.TakeOwnership(RegistryHelper.AllowedDMA);
                RegistryHelper.TakeOwnership(RegistryHelper.UnallowedDMA);
            
                // Initialize DMA list
                RegistryHelper.ListAllowedDMA();
                RegistryHelper.ListUnallowedDMA();
            
                // Match list
                PCIHelper.MatchPCIID();

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        private static string SelectOption()
        {
            Console.WriteLine("Options:\r\n\t" +
                              "A = Add all to allow list\r\n\t" +
                              "D = Add all to disallow list\r\n\t" +
                              "X = Exit program\r\n\t" +
                              "Type device # to modify specific device in the list");
            Console.WriteLine("Input:");
            var read = Console.ReadLine()!;
            if (int.TryParse(read, out int value))
            {
                return value.ToString();
            }

            return read;
        }

        private static void ModifyDevice(int devNum)
        {
            List<PCIDeviceList> pciDevList = PCIDevice.PciDevice;

            var device = pciDevList[devNum];
            Console.WriteLine("Selected device:\r\n" +
                              $"{device.N}, Match = {device.Match}, {device.DeviceID}, {device.Description}");
            Console.WriteLine("Action:" +
                              "A = Add to allow list\r\n\t" +
                              "D = Add to disallow list\r\n\t" +
                              "X = Go back to main menu\r\n\t");
            var read = Console.ReadLine();
            switch (read!.ToUpper())
            {
                case "A":
                    RegistryHelper.AddToAllowList(devNum);
                    _isInitialized = null;
                    break;
                case "D":
                    RegistryHelper.AddToUnallowList(devNum);
                    _isInitialized = null;
                    break;
            }
            
            // Clear console and go back to main
            Console.Clear();
            Main();
        }


        
        
    }
}


