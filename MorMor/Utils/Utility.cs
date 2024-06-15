using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Terraria;

namespace MorMor.Utils;

public class Utility
{
    
    public static Item GetItemById(int id)
    {
        var item = new Item();
        item.netDefaults(id);
        return item;
    }

    
    public static List<Item> GetItemByIdOrName(string text)
	{
		int type = -1;
		if (Int32.TryParse(text, out type))
		{
			if (type >= Terraria.ID.ItemID.Count)
				return new List<Item>();
			return new List<Item> { GetItemById(type) };
		}
		Item item = GetItemFromTag(text);
		if (item != null)
			return new List<Item>() { item };
		return GetItemByName(text);
	}
		
	public static Item GetItemFromTag(string tag)
	{
		Regex regex = new Regex(@"\[i(tem)?(?:\/s(?<Stack>\d{1,4}))?(?:\/p(?<Prefix>\d{1,3}))?:(?<NetID>-?\d{1,4})\]");
		Match match = regex.Match(tag);
		if (!match.Success)
			return null;
		Item item = new Item();
		item.netDefaults(Int32.Parse(match.Groups["NetID"].Value));
		if (!String.IsNullOrWhiteSpace(match.Groups["Stack"].Value))
			item.stack = Int32.Parse(match.Groups["Stack"].Value);
		if (!String.IsNullOrWhiteSpace(match.Groups["Prefix"].Value))
			item.prefix = Byte.Parse(match.Groups["Prefix"].Value);
		return item;
	}
    
    public static List<Item> GetItemByName(string name)
	{
		var startswith = new List<int>();
		var contains = new List<int>();
		for (int i = 1; i < Terraria.ID.ItemID.Count; i++)
		{
			var currentName = Lang.GetItemNameValue(i);
			if (!string.IsNullOrEmpty(currentName))
			{
				if (currentName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					return new List<Item> { GetItemById(i) };
				if (currentName.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))
				{
					startswith.Add(i);
					continue;
				}
				if (currentName.Contains(name, StringComparison.InvariantCultureIgnoreCase))
				{
					contains.Add(i);
					continue;
				}
			}
		}

		if (startswith.Count != 1)
			startswith.AddRange(contains);
		return startswith.Select(GetItemById).ToList();
	}

    [DllImport("psapi.dll")]
    private static extern bool EmptyWorkingSet(IntPtr lpAddress);

    public static void FreeMemory()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        foreach (var process in Process.GetProcesses())
        {
            if ((process.ProcessName == "System") && (process.ProcessName == "Idle"))
                continue;
            try
            {
                EmptyWorkingSet(process.Handle);
            }
            catch { }
        }
    }

    #region ����ڴ���ϢAPI
    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GlobalMemoryStatusEx(ref MEMORY_INFO mi);

    //�����ڴ����Ϣ�ṹ
    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORY_INFO
    {
        public uint dwLength; //��ǰ�ṹ���С
        public uint dwMemoryLoad; //��ǰ�ڴ�ʹ����
        public ulong ullTotalPhys; //�ܼ������ڴ��С
        public ulong ullAvailPhys; //���������ڴ��С
        public ulong ullTotalPageFile; //�ܼƽ����ļ���С
        public ulong ullAvailPageFile; //�ܼƽ����ļ���С
        public ulong ullTotalVirtual; //�ܼ������ڴ��С
        public ulong ullAvailVirtual; //���������ڴ��С
        public ulong ullAvailExtendedVirtual; //���� ���ֵʼ��Ϊ0
    }
    #endregion

    #region ��ʽ��������С
    /// <summary>
    /// ��ʽ��������С
    /// </summary>
    /// <param name="size">������B��</param>
    /// <returns>�Ѹ�ʽ��������</returns>
    public static string FormatSize(double size)
    {
        double d = (double)size;
        int i = 0;
        while ((d > 1024) && (i < 5))
        {
            d /= 1024;
            i++;
        }
        string[] unit = { "B", "KB", "MB", "GB", "TB" };
        return (string.Format("{0} {1}", Math.Round(d, 2), unit[i]));
    }
    #endregion

    #region ��õ�ǰ�ڴ�ʹ�����
    /// <summary>
    /// ��õ�ǰ�ڴ�ʹ�����
    /// </summary>
    /// <returns></returns>
    public static MEMORY_INFO GetMemoryStatus()
    {
        MEMORY_INFO mi = new MEMORY_INFO();
        mi.dwLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf(mi);
        GlobalMemoryStatusEx(ref mi);
        return mi;
    }
    #endregion

    #region ��õ�ǰ���������ڴ��С
    /// <summary>
    /// ��õ�ǰ���������ڴ��С
    /// </summary>
    /// <returns>��ǰ���������ڴ棨B��</returns>
    public static ulong GetAvailPhys()
    {
        MEMORY_INFO mi = GetMemoryStatus();
        return mi.ullAvailPhys;
    }
    #endregion

    #region ��õ�ǰ��ʹ�õ��ڴ��С
    /// <summary>
    /// ��õ�ǰ��ʹ�õ��ڴ��С
    /// </summary>
    /// <returns>��ʹ�õ��ڴ��С��B��</returns>
    public static ulong GetUsedPhys()
    {
        MEMORY_INFO mi = GetMemoryStatus();
        return (mi.ullTotalPhys - mi.ullAvailPhys);
    }
    #endregion

    #region ��õ�ǰ�ܼ������ڴ��С
    /// <summary>
    /// ��õ�ǰ�ܼ������ڴ��С
    /// </summary>
    /// <returns&gt;�ܼ������ڴ��С��B��&lt;/returns&gt;
    public static ulong GetTotalPhys()
    {
        MEMORY_INFO mi = GetMemoryStatus();
        return mi.ullTotalPhys;
    }
    #endregion

    public static void KillChrome()
    {
        foreach (var process in Process.GetProcesses())
        {
            if (process.ProcessName.Contains("chrome"))
            {
                process.Kill();
            }
        }
    }

	public static async Task<Stream> Markdown(string md)
	{ 
		return await MarkdownHelper.ToImage(md);
	}
}
