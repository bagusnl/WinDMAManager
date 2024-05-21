using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Win32;
using WinDMAManager.List;
using static CosmosKey.Utils.TokenManipulator;

namespace WinDMAManager.Helper;

public class RegistryHelper
{
    public const string AllowedDMA =
        @"SYSTEM\CurrentControlSet\Control\DmaSecurity\AllowedBuses";

    public const string  UnallowedDMA = @"SYSTEM\CurrentControlSet\Control\DmaSecurity\UnallowedBuses";
    
    
    //https://stackoverflow.com/questions/38448390/how-do-i-programmatically-give-ownership-of-a-registry-key-to-administrators
    public static Task TakeOwnership(string reg)
    {
        try
        {
            // Manipulate token to gain access
            AddPrivilege(SE_TAKE_OWNERSHIP_NAME);
            AddPrivilege(SE_RESTORE_NAME);
            
            var key = Registry.LocalMachine.OpenSubKey(reg, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.TakeOwnership);
            if (key == null) throw new NullReferenceException("Key is uninitialized!");
            
            var curUser = new NTAccount(WindowsIdentity.GetCurrent().Name);
            var ac = key.GetAccessControl();
            //Console.WriteLine($"Current owner: {ac.GetOwner(typeof(NTAccount))}");
            // Console.WriteLine(ac.GetAccessRules(true, true, typeof(NTAccount)).ToString());
            ac.SetOwner(curUser);
            key.SetAccessControl(ac);
            
            ac.AddAccessRule(new RegistryAccessRule(curUser, RegistryRights.TakeOwnership, AccessControlType.Allow));
            key.SetAccessControl(ac);

            ac.AddAccessRule(new RegistryAccessRule(curUser, RegistryRights.FullControl, AccessControlType.Allow));
            key.SetAccessControl(ac);
            
            return Task.CompletedTask;
        }
        finally
        {
            RemovePrivilege(SE_TAKE_OWNERSHIP_NAME);
            RemovePrivilege(SE_RESTORE_NAME);
        }
    }

    public static void ListAllowedDMA()
    {
        var key = Registry.LocalMachine.OpenSubKey(AllowedDMA, RegistryRights.ReadKey);
        if (key == null) throw new NullReferenceException("AllowedBusses Subkey is uninitialized!");

        var N = 0;
        
        foreach (var keyName in key.GetValueNames())
        {
            // skip default
            if (keyName == "") continue;

            var kind = key.GetValueKind(keyName);
            if (kind == RegistryValueKind.String)
            {
                var data = key.GetValue(keyName)!.ToString()!;
                AllowedDMAList.AddItem(N, keyName, data);
            }
        }
    }
    
    public static void ListUnallowedDMA()
    {
        var key = Registry.LocalMachine.OpenSubKey(UnallowedDMA, RegistryRights.ReadKey);
        if (key == null) throw new NullReferenceException("UnallowedBusses Subkey is uninitialized!");

        var N = 0;
        
        foreach (var keyName in key.GetValueNames())
        {
            // skip default
            if (keyName == "") continue;

            var kind = key.GetValueKind(keyName);
            if (kind == RegistryValueKind.String)
            {
                var data = key.GetValue(keyName)!.ToString()!;
                UnallowedDMAList.AddItem(N, keyName, data);
            }
        }
    }
    
    public static void AddToAllowList(int devNum)
    {
        try
        {
            List<PCIDeviceList> pciDevList = PCIDevice.PciDevice;

            var device = pciDevList[devNum];
            var allowList = Registry.LocalMachine.OpenSubKey(AllowedDMA, RegistryKeyPermissionCheck.ReadWriteSubTree);

            var desc = device.Description;
            var devId = device.DeviceID;
            allowList!.SetValue(desc, devId, RegistryValueKind.String);
            Console.WriteLine($"Added {desc} to allowed DMA list! Press any key to continue...");
            Console.ReadKey();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public static void AddToUnallowList(int devNum)
    {
        try
        {
            List<PCIDeviceList> pciDevList = PCIDevice.PciDevice;

            var device = pciDevList[devNum];
            var unallowList =
                Registry.LocalMachine.OpenSubKey(UnallowedDMA, RegistryKeyPermissionCheck.ReadWriteSubTree);

            var desc = device.Description;
            var devId = device.DeviceID;
            unallowList!.SetValue(desc, devId, RegistryValueKind.String);
            Console.WriteLine($"Added {desc} to unallowed DMA list! Press any key to continue...");
            Console.ReadKey();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static void ResetAllowList()
    {
        var key = Registry.LocalMachine.OpenSubKey(AllowedDMA, true);
        if (key == null) throw new NullReferenceException("AllowedDMA key is uninitialized!");

        var valueNames = key.GetValueNames();
        foreach (var valueName in valueNames)
        {
            if (valueName == "")
            {
                continue;
            }
            
            key.DeleteValue(valueName);
            Console.WriteLine($"Deleted {valueName}");
        }
    }
    
    public static void ResetUnallowList()
    {
        var key = Registry.LocalMachine.OpenSubKey(UnallowedDMA, true);
        if (key == null) throw new NullReferenceException("UnallowedDMA key is uninitialized!");

        var valueNames = key.GetValueNames();
        foreach (var valueName in valueNames)
        {
            if (valueName == "")
            {
                continue;
            }
            
            key.DeleteValue(valueName);
            Console.WriteLine($"Deleted {valueName}");
        }
    }
}