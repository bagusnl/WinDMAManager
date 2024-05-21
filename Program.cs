
using WinDMAManager.Helper;
using WinDMAManager.Helper.PCIDevices;
using WinDMAManager.List;

namespace WinDMAManager
{
    internal static class Program
    {
        private static bool? _isInitialized;
        
        private static void Main()
        {
            // Run initialization if flag is null
            _isInitialized ??= Initialize();
            if ((bool)!_isInitialized)
            {
                Console.WriteLine("Failure to initialize! Exiting app...");
                Environment.Exit(100);
            }
            
            Console.WriteLine("Match List:\r\n\t" +
                              "Y = In allowed list\r\n\t" +
                              "N = In unallowed list\r\n\t" +
                              "? = Not found in any list");
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
                case "Q":
                    Console.WriteLine("Are you sure? [Y/N]");
                    var confA = Console.ReadLine()!.ToUpper();
                    if (confA == "Y")
                    {
                        RegistryHelper.ResetAllowList();
                        Deinitialize();
                    }
                    break;
                case "E":
                    Console.WriteLine("Are you sure? [Y/N]");
                    var confD = Console.ReadLine()!.ToUpper();
                    if (confD == "Y")
                    {
                        RegistryHelper.ResetUnallowList();
                        Deinitialize();
                    }
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

            Main();
        }

        private static bool Initialize()
        {
            try
            {
                // Initialize PCI list
                PCIHelper.SearchAllPCIDevice();
            
                // Takeover DMA registry
                var takeOwnAllowDMA = RegistryHelper.TakeOwnership(RegistryHelper.AllowedDMA);
                if (takeOwnAllowDMA != Task.CompletedTask)
                    throw new ApplicationException("Failed to take ownership on AllowedDMA registry!");
                var takeOwnUnallowDMA = RegistryHelper.TakeOwnership(RegistryHelper.UnallowedDMA);
                if (takeOwnUnallowDMA != Task.CompletedTask)
                    throw new ApplicationException("Failed to take ownership on UnallowedDMA registry!");
            
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

        private static void Deinitialize()
        {
            AllowedDMA.DMADevice.Clear();
            UnallowedDMA.DMADevice.Clear();
            PCIDevice.PciDevice.Clear();

            _isInitialized = null;
        }
        
        private static string SelectOption()
        {
            Console.WriteLine("Options:\r\n\t" +
                              "A = Add all to allow list\r\n\t" +
                              "D = Add all to disallow list\r\n\t" +
                              "Q = Reset allowed DMA list\r\n\t" +
                              "E = Reset unallowed DMA list\r\n\t" +
                              "X = Exit program\r\n\t" +
                              "Type device # to modify specific device in the list");
            Console.WriteLine("Input:");
            return Console.ReadLine()!;
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
                    Deinitialize();
                    break;
                case "D":
                    RegistryHelper.AddToUnallowList(devNum);
                    Deinitialize();
                    break;
            }
            
            // Clear console and go back to main
            Console.Clear();
            Main();
        }


        
        
    }
}


