using GUPS.AntiCheat.Protected;

public class TUIStashUpdateInfo
{
	public int level;

	public TUIPriceInfo price_info;

	public ProtectedInt32 max_capacity;

	public string introduce = string.Empty;

	public TUIStashUpdateInfo(int m_level, TUIPriceInfo m_price, ProtectedInt32 m_max_capacity, string m_introduce)
	{
		level = m_level;
		price_info = m_price;
		max_capacity = m_max_capacity;
		introduce = m_introduce;
	}
}
