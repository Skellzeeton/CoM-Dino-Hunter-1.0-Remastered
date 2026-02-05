using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class IAPPlugin
{
	public enum Status
	{
		kUserCancel = -2,
		kError = -1,
		kBuying = 0,
		kSuccess = 1
	}

	
	protected static extern void PurchaseAddProductId(string productId);

	
	protected static extern void PurchaseLoadProductInfo();

	
	protected static extern bool PurchaseGetProductInfoCurrency(string productId, [Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder currency);

	
	protected static extern bool PurchaseGetProductInfoPrice(string productId, ref float price);

	
	protected static extern void PurchaseProduct(string productId, string productCount);

	
	protected static extern int PurchaseStatus();

	
	protected static extern void PurchaseGetTransactionIdentifier([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer);

	
	protected static extern void PurchaseGetTransactionReceipt([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder buffer);

	
	protected static extern void RestoreProduct();

	
	protected static extern int RestoreStatus();

	
	protected static extern void RestoreGetProductId([Out][MarshalAs(UnmanagedType.LPStr)] StringBuilder productId);

	public static void AddProductId(string productId)
	{
		PurchaseAddProductId(productId);
	}

	public static void LoadProductInfo()
	{
		PurchaseLoadProductInfo();
	}

	public static string GetProductInfoCurrency(string productId)
	{
		string result = string.Empty;
		StringBuilder stringBuilder = new StringBuilder(256);
		if (PurchaseGetProductInfoCurrency(productId, stringBuilder))
		{
			result = stringBuilder.ToString();
		}
		return result;
	}

	public static float GetProductInfoPrice(string productId)
	{
		float price = 0f;
		PurchaseGetProductInfoPrice(productId, ref price);
		return price;
	}

	public static void NowPurchaseProduct(string productId, string productCount)
	{
		if (MiscPlugin.IsIAPCrack())
		{
			Debug.Log("IsIAPCrack!!!!!!");
		}
		else
		{
			PurchaseProduct(productId, productCount);
		}
	}

	public static int GetPurchaseStatus()
	{
		if (MiscPlugin.IsIAPCrack())
		{
			return -3;
		}
		return PurchaseStatus();
	}

	public static string GetTransactionIdentifier()
	{
		string empty = string.Empty;
		StringBuilder stringBuilder = new StringBuilder(4096);
		PurchaseGetTransactionIdentifier(stringBuilder);
		return stringBuilder.ToString();
	}

	public static string GetTransactionReceipt()
	{
		string empty = string.Empty;
		StringBuilder stringBuilder = new StringBuilder(4096);
		PurchaseGetTransactionReceipt(stringBuilder);
		return stringBuilder.ToString();
	}

	public static void DoRestoreProduct()
	{
		RestoreProduct();
	}

	public static int DoRestoreStatus()
	{
		int num = 1;
		return RestoreStatus();
	}

	public static string[] DoRestoreGetProductId()
	{
		string empty = string.Empty;
		StringBuilder stringBuilder = new StringBuilder(1024);
		RestoreGetProductId(stringBuilder);
		empty = stringBuilder.ToString();
		return empty.Split('|');
	}
}
