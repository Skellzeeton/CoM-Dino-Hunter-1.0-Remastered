using System.Collections.Generic;
using UnityEngine;

public class iSpawnBullet : MonoBehaviour
{
	public float fAtkRange;

	public int nEffHit = -1;

	public int nEffHitGround = -1;

	public int nHitMask = -1;

	public Vector3 v3RotateSpeed = new Vector3(0f, 0f, 0f);

	public string sAudioHit = string.Empty;

	public CWeaponInfoLevel m_pWeaponLvlInfo;

	protected iGameSceneBase m_GameScene;

	protected iGameLogic m_GameLogic;

	protected Transform m_Transform;

	protected int m_nOwnerUID;

	protected int[] m_arrFunc;

	protected int[] m_arrValueX;

	protected int[] m_arrValueY;

	protected bool m_bActive;

	protected Vector3 m_v3VelocityBase;

	protected ParticleSystem[] m_arrParicleSystem;

	protected bool m_bEmission;

	protected HashSet<int> m_HitTargets;
	private const int EFF_FATAL_BOSS = 1904;
	private const int EFF_FATAL_MOB  = 1905;

	public int Owner
	{
		get
		{
			return m_nOwnerUID;
		}
	}

	public void Awake()
	{
		m_GameScene = iGameApp.GetInstance().m_GameScene;
		m_GameLogic = m_GameScene.GetGameLogic();
		m_Transform = base.transform;
		m_bActive = false;
		m_arrFunc = new int[3];
		m_arrValueX = new int[3];
		m_arrValueY = new int[3];
		m_arrParicleSystem = GetComponentsInChildren<ParticleSystem>();
		if (m_arrParicleSystem != null)
		{
			ParticleSystem[] arrParicleSystem = m_arrParicleSystem;
			foreach (ParticleSystem particleSystem in arrParicleSystem)
			{
				particleSystem.enableEmission = false;
			}
		}
		m_bEmission = false;
		m_HitTargets = new HashSet<int>();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!m_bActive)
		{
			return;
		}
		if (!m_bEmission)
		{
			m_bEmission = true;
			if (m_arrParicleSystem != null)
			{
				ParticleSystem[] arrParicleSystem = m_arrParicleSystem;
				foreach (ParticleSystem particleSystem in arrParicleSystem)
				{
					particleSystem.enableEmission = true;
				}
			}
		}
		if (m_GameScene != null && !m_GameScene.isPause)
		{
			if (v3RotateSpeed != Vector3.zero)
			{
				m_Transform.rotation *= new Quaternion(v3RotateSpeed.x * Time.deltaTime, v3RotateSpeed.y * Time.deltaTime, v3RotateSpeed.z * Time.deltaTime, 1f);
			}
			OnUpdate(Time.deltaTime);
			if (m_Transform.position.y <= -500f)
			{
				m_bActive = false;
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void OnTriggerEnter(Collider collider)
	{
		if (m_GameScene == null || m_GameScene.isPause)
		{
			return;
		}
		CCharBase cCharBase = m_GameScene.GetPlayer(m_nOwnerUID);
		if (cCharBase == null)
		{
			cCharBase = m_GameScene.GetMob(m_nOwnerUID);
			if (cCharBase == null)
			{
				return;
			}
		}
		iSpawnBullet component = collider.transform.root.GetComponent<iSpawnBullet>();
		if (component != null && component.Owner == m_nOwnerUID)
		{
			return;
		}
		if (fAtkRange <= 0f)
		{
			CCharBase component2 = collider.transform.root.GetComponent<CCharBase>();
			if (component2 != null && !component2.isDead && cCharBase != component2 && !cCharBase.IsAlly(component2))
			{
				if (m_HitTargets.Contains(component2.UID))
				{
					return;
				}
				Vector3 v3HitDir = collider.transform.position - m_Transform.position;
				Vector3 position = m_Transform.position;
				if (m_GameScene.IsMyself(cCharBase) || (cCharBase.IsMonster() && m_GameScene.IsRoomMaster()))
				{
					if (component2.IsMob() || component2.IsBoss())
					{
						((CCharMob)component2).SetLifeBarParam(1f);
					}
					if (m_pWeaponLvlInfo != null)
					{
						CCharPlayer cCharPlayer = cCharBase as CCharPlayer;
						if (cCharPlayer != null)
						{
							float num = cCharPlayer.CalcWeaponDamage(m_pWeaponLvlInfo);
							float num2 = cCharPlayer.CalcCritical(m_pWeaponLvlInfo);
							float num3 = cCharPlayer.CalcCriticalDmg(m_pWeaponLvlInfo);
							bool bCritical = false;
							if (num2 > Random.Range(1f, 100f))
							{
								num *= 1f + num3 / 100f;
								bCritical = true;
							}
							float elementValue = m_pWeaponLvlInfo.GetElementValue(component2.ID);
							if (elementValue != 0f)
							{
								num *= 1f + elementValue / 100f;
							}
							float num4 = component2.CalcProtect();
							num *= 1f - num4 / 100f;
							component2.OnHit(0f - num, m_pWeaponLvlInfo, string.Empty);
							if (m_GameScene.IsMyself(component2))
							{
								m_GameScene.AddDamageText(num, component2.GetBone(1).position, Color.red, bCritical);
							}
							else
							{
								m_GameScene.AddDamageText(num, component2.GetBone(1).position, bCritical);
							}
							m_GameScene.AddHitEffect(component2.GetBone(1).position, Vector3.forward, 1116);
						}
					}
					iGameLogic.HitInfo hitinfo = new iGameLogic.HitInfo();
					hitinfo.v3HitDir = v3HitDir;
					hitinfo.v3HitPos = position;
					m_GameLogic.CaculateFunc(cCharBase, component2, m_arrFunc, m_arrValueX, m_arrValueY, ref hitinfo);
					m_HitTargets.Add(component2.UID);
					if (component2.isDead)
					{
						int fatalEff = component2.IsBoss() ? EFF_FATAL_BOSS : EFF_FATAL_MOB;
						m_GameScene.AddEffect(component2.GetBone(1).position, Vector3.forward, 4.25f, fatalEff);
						CCharMob cCharMob = component2 as CCharMob;
						CCharUser cCharUser = cCharBase as CCharUser;
						if (cCharUser != null && cCharMob != null)
						{
							CMobInfoLevel mobInfo = cCharMob.GetMobInfo();
							if (mobInfo != null)
							{
								int num5 = mobInfo.nExp;
								float value = cCharUser.Property.GetValue(kProEnum.Char_IncreaseExp);
								if (value > 0f)
								{
									num5 = (int)((float)num5 * (1f + value / 100f));
								}
								cCharUser.AddExp(num5);
								m_GameScene.AddExpText(num5, hitinfo.v3HitPos);
							}
						}
					}
				}
				component2.PlayAudio(kAudioEnum.HitBody);
				m_GameScene.PlayAudio(m_Transform.position, sAudioHit);
				m_GameScene.AddEffect(m_Transform.position, Vector3.forward, 2f, nEffHit);
				Object.Destroy(base.gameObject);
			}
			else if (collider.gameObject.layer == 29 && IsCanHitFloor())
			{
				Vector3 position2 = m_Transform.position;
				position2.y += 100f;
				RaycastHit hitInfo;
				if (Physics.Raycast(new Ray(position2, Vector3.down), out hitInfo, 1000f, 536870912))
				{
					if (nHitMask != -1)
					{
						m_GameScene.AddEffect(hitInfo.point + new Vector3(0f, 0.01f, 0f), Vector3.forward, 2f, nHitMask);
					}
					OnHitGround(hitInfo.point);
				}
				m_GameScene.PlayAudio(m_Transform.position, sAudioHit);
				m_GameScene.AddEffect(m_Transform.position, Vector3.forward, 2f, nEffHitGround);
				Object.Destroy(base.gameObject);
			}
			else if (collider.gameObject.layer == 31)
			{
				m_GameScene.PlayAudio(m_Transform.position, sAudioHit);
				m_GameScene.AddEffect(m_Transform.position, Vector3.forward, 2f, nEffHit);
				Object.Destroy(base.gameObject);
			}
			return;
		}
		if (cCharBase.IsMonster())
		{
			if ((collider.gameObject.layer != 31 && collider.gameObject.layer != 29) || (collider.gameObject.layer == 29 && !IsCanHitFloor()))
			{
				return;
			}
		}
		else if (collider.gameObject.layer != 31 && collider.gameObject.layer != 29 && collider.gameObject.layer != 26)
		{
			return;
		}
		if (m_GameScene.IsMyself(cCharBase) || (cCharBase.IsMonster() && m_GameScene.IsRoomMaster()))
		{
			List<CCharBase> unitList = m_GameScene.GetUnitList();
			CCharBase component3 = collider.transform.root.GetComponent<CCharBase>();
			foreach (CCharBase item in unitList)
			{
				if (item == null || item.isDead || cCharBase.IsAlly(item) || (component3 != item && Vector3.Distance(m_Transform.position, item.Pos) > fAtkRange))
				{
					continue;
				}
				if (m_HitTargets.Contains(item.UID))
				{
					continue;
				}

				if (item.IsMob() || item.IsBoss())
				{
					((CCharMob)item).SetLifeBarParam(1f);
				}
				if (m_pWeaponLvlInfo != null)
				{
					CCharPlayer cCharPlayer2 = cCharBase as CCharPlayer;
					if (cCharPlayer2 != null)
					{
						float num6 = cCharPlayer2.CalcWeaponDamage(m_pWeaponLvlInfo);
						float num7 = cCharPlayer2.CalcCritical(m_pWeaponLvlInfo);
						float num8 = cCharPlayer2.CalcCriticalDmg(m_pWeaponLvlInfo);
						bool bCritical2 = false;
						if (num7 > Random.Range(1f, 100f))
						{
							num6 *= 1f + num8 / 100f;
							bCritical2 = true;
						}
						float elementValue2 = m_pWeaponLvlInfo.GetElementValue(item.ID);
						if (elementValue2 != 0f)
						{
							num6 *= 1f + elementValue2 / 100f;
						}
						float num9 = item.CalcProtect();
						num6 *= 1f - num9 / 100f;
						item.OnHit(0f - num6, m_pWeaponLvlInfo, string.Empty);
						if (m_GameScene.IsMyself(item))
						{
							m_GameScene.AddDamageText(num6, item.GetBone(1).position, Color.red, bCritical2);
						}
						else
						{
							m_GameScene.AddDamageText(num6, item.GetBone(1).position, bCritical2);
						}
						m_GameScene.AddHitEffect(item.GetBone(1).position, Vector3.forward, 1116);
						m_HitTargets.Add(item.UID);
					}
				}
				iGameLogic.HitInfo hitinfo2 = new iGameLogic.HitInfo();
				hitinfo2.v3HitDir = m_Transform.position - item.Pos;
				hitinfo2.v3HitPos = item.GetBone(1).position;
				m_GameLogic.CaculateFunc(cCharBase, item, m_arrFunc, m_arrValueX, m_arrValueY, ref hitinfo2);
				if (item.isDead)
				{
					int fatalEff = item.IsBoss() ? EFF_FATAL_BOSS : EFF_FATAL_MOB;
					m_GameScene.AddEffect(item.GetBone(1).position, Vector3.forward, 4f, fatalEff);
					CCharMob cCharMob2 = item as CCharMob;
					CCharUser cCharUser2 = cCharBase as CCharUser;
					if (cCharUser2 != null && cCharMob2 != null)
					{
						CMobInfoLevel mobInfo2 = cCharMob2.GetMobInfo();
						if (mobInfo2 != null)
						{
							int num10 = mobInfo2.nExp;
							float value2 = cCharUser2.Property.GetValue(kProEnum.Char_IncreaseExp);
							if (value2 > 0f)
							{
								num10 = (int)((float)num10 * (1f + value2 / 100f));
							}
							cCharUser2.AddExp(num10);
							m_GameScene.AddExpText(num10, hitinfo2.v3HitPos);
						}
						cCharMob2.m_v3DeadDirection = item.Pos - m_Transform.position;
						cCharMob2.m_v3DeadDirection.y = 0f;
						cCharMob2.m_v3DeadDirection.Normalize();
					}
				}
				item.PlayAudio(kAudioEnum.HitBody);
			}
			if (component3 != null)
			{
				m_GameScene.PlayAudio(m_Transform.position, sAudioHit);
				m_GameScene.AddEffect(m_Transform.position, Vector3.forward, 2f, nEffHit);
				Object.Destroy(base.gameObject);
				return;
			}
		}
		if (collider.gameObject.layer == 29 && IsCanHitFloor())
		{
			Vector3 position3 = m_Transform.position;
			position3.y += 100f;
			RaycastHit hitInfo2;
			if (Physics.Raycast(new Ray(position3, Vector3.down), out hitInfo2, 1000f, 536870912))
			{
				if (nHitMask != -1)
				{
					m_GameScene.AddEffect(hitInfo2.point + new Vector3(0f, 0.01f, 0f), Vector3.forward, 2f, nHitMask);
				}
				OnHitGround(hitInfo2.point);
			}
			m_GameScene.PlayAudio(m_Transform.position, sAudioHit);
			m_GameScene.AddEffect(m_Transform.position, Vector3.forward, 2f, nEffHitGround);
			Object.Destroy(base.gameObject);
		}
		else
		{
			m_GameScene.PlayAudio(m_Transform.position, sAudioHit);
			m_GameScene.AddEffect(m_Transform.position, Vector3.forward, 2f, nEffHit);
			Object.Destroy(base.gameObject);
		}
	}

	public virtual void Initialize(int nUID, int nID, Vector3 v3Pos, Vector3 v3Force, int[] arrFunc, int[] arrValueX, int[] arrValueY)
	{
		m_nOwnerUID = nUID;
		m_v3VelocityBase = v3Force;
		for (int i = 0; i < 3; i++)
		{
			m_arrFunc[i] = arrFunc[i];
			m_arrValueX[i] = arrValueX[i];
			m_arrValueY[i] = arrValueY[i];
		}
		m_Transform.position = v3Pos;
		m_Transform.forward = v3Force;
		m_bActive = true;
		OnInit();
		if (m_HitTargets != null)
		{
			m_HitTargets.Clear();
		}
	}

	public virtual void SetForce(Vector3 v3Force)
	{
		m_v3VelocityBase = v3Force;
	}

	public virtual bool IsCanHitFloor()
	{
		return true;
	}

	protected virtual void OnInit()
	{
	}

	protected virtual void OnUpdate(float deltaTime)
	{
		if (!(m_Transform == null))
		{
			m_Transform.position += m_v3VelocityBase * Time.deltaTime;
		}
	}

	protected virtual void OnHitGround(Vector3 v3Hit)
	{
	}
}