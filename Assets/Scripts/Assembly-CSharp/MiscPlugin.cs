using System.Runtime.InteropServices;
using System.Text;

public class MiscPlugin
{
	
	protected static extern void OSGetMAC([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder output);

	public static string GetMacAddr()
	{
		StringBuilder stringBuilder = new StringBuilder(256);
		OSGetMAC(stringBuilder);
		return stringBuilder.ToString();
	}

	
	protected static extern bool OSIsIAPCrack();

	public static bool IsIAPCrack()
	{
		return OSIsIAPCrack();
	}

	
	protected static extern bool OSIsJailbreak();

	public static bool IsJailbreak()
	{
		return OSIsJailbreak();
	}
}
