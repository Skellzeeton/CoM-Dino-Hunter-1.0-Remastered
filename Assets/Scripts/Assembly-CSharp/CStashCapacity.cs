using GUPS.AntiCheat.Protected;

public class CStashCapacity
{
	public ProtectedInt32 nLevel;

	public bool isCrystalPurchase;

	public ProtectedInt32 nPrice;

	public ProtectedInt32 nCapacity;

	public string sLevelUpDesc = string.Empty;

	public CStashCapacity(ProtectedInt32 level, bool iscrystalpurchase, ProtectedInt32 price, ProtectedInt32 capacity, string lvlupdesc)
	{
		nLevel = level;
		isCrystalPurchase = iscrystalpurchase;
		nPrice = price;
		nCapacity = capacity;
		sLevelUpDesc = lvlupdesc;
	}
}
