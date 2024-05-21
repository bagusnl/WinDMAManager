
https://canary.discord.com/channels/296120206932312064/1139368007995510885/1241993871672606780
1. take ownership HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\DmaSecurity\AllowedBuses
2. kasih user full control ke regkey itu
3. add string values ke regkey, masukin nama serah terus valuenya sesuai compatible IDs

https://canary.discord.com/channels/296120206932312064/1139368007995510885/1242007544524705802
1. list of PCIE devices: wmi class `Win32_PnpEntity`, find all with `PCI` in the first 3 char of `queryObj["PNPDeviceID"]`, device name is on `queryObj["Name"]`
   Doc: <https://stackoverflow.com/questions/11966684/how-to-get-the-device-id-of-pci-devices>
2. take registry ownership <https://stackoverflow.com/questions/38448390/how-do-i-programmatically-give-ownership-of-a-registry-key-to-administrators>