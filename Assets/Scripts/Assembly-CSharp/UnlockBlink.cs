using UnityEngine;

public class UnlockBlink : MonoBehaviour
{
	public GameObject go_blink;

	public TUIMeshSprite img_texture;

	public TUILabel label_text;

	private bool open_blink;

	private float fade_time = 0.5f;

	private float now_time;

	private string weapon_path = "Artist/Textures/Weapon/";

	private string skill_path = "TUI/Skill/";

	private void Start()
	{
	}

	private void Update()
	{
		if (open_blink)
		{
			now_time += Time.deltaTime;
			if (now_time < fade_time)
			{
				go_blink.transform.localScale = new Vector3(now_time * 4f, now_time * 4f, 1f);
			}
			go_blink.transform.localEulerAngles += new Vector3(0f, 0f, -1f);
		}
	}

	public void OpenBlinkWeapon(TUIMeshSprite m_sprite, string m_text)
	{
		open_blink = true;
		base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		img_texture.UseCustomize = true;
		img_texture.CustomizeTexture = m_sprite.CustomizeTexture;
		img_texture.CustomizeRect = m_sprite.CustomizeRect;
		if (img_texture.GetComponent<Animation>() != null)
		{
			img_texture.GetComponent<Animation>().Play();
		}
		label_text.Text = m_text;
	}

	public void OpenBlinkWeapon(int m_id, string m_text, bool m_use_customize = false)
	{
		open_blink = true;
		transform.localPosition = new Vector3(0f, 0f, transform.localPosition.z);

		string weaponTexture = TUIMappingInfo.Instance().GetWeaponTexture(m_id);
		if (string.IsNullOrEmpty(weaponTexture))
		{
			Debug.LogWarning("Missing weapon texture mapping for id: " + m_id);
			return;
		}
		SetCustomizeTexture(img_texture, weapon_path + weaponTexture);
		if (img_texture.GetComponent<Animation>() != null)
		{
			img_texture.GetComponent<Animation>().Play();
		}

		label_text.Text = m_text;
	}


	public void OpenBlinkSkill(int m_id, string m_text, bool m_use_customize = false)
	{
		open_blink = true;
		base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		string skillTexture = TUIMappingInfo.Instance().GetSkillTexture(m_id);
		if (m_use_customize)
		{
			SetCustomizeTexture(img_texture, skill_path + skillTexture);
		}
		else if (img_texture != null)
		{
			img_texture.texture = skillTexture;
		}
		if (img_texture.GetComponent<Animation>() != null)
		{
			img_texture.GetComponent<Animation>().Play();
		}
		label_text.Text = m_text;
	}

	public void OpenBlinkRole(int m_id, string m_text)
	{
		open_blink = true;
		base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		string roleTexture = TUIMappingInfo.Instance().GetRoleTexture(m_id);
		img_texture.texture = roleTexture;
		if (img_texture.GetComponent<Animation>() != null)
		{
			img_texture.GetComponent<Animation>().Play();
		}
		label_text.Text = m_text;
	}

	public void OpenBlinkSkill(string m_texture_name, string m_text, bool m_use_customize = false)
	{
		open_blink = true;
		base.transform.localPosition = new Vector3(0f, 0f, base.transform.localPosition.z);
		if (m_use_customize)
		{
			SetCustomizeTexture(img_texture, skill_path + m_texture_name);
		}
		else
		{
			img_texture.texture = m_texture_name;
		}
		if (img_texture.GetComponent<Animation>() != null)
		{
			img_texture.GetComponent<Animation>().Play();
		}
		label_text.Text = m_text;
	}

	public void CloseBlink()
	{
		open_blink = false;
		now_time = 0f;
		base.transform.localPosition = new Vector3(0f, -1000f, base.transform.localPosition.z);
		img_texture.UseCustomize = false;
		img_texture.CustomizeTexture = null;
		img_texture.CustomizeRect = new Rect(0f, 0f, 0f, 0f);
		img_texture.texture = string.Empty;
	}

	public void SetCustomizeTexture(TUIMeshSprite m_sprite, string m_path)
	{
		m_sprite.texture = string.Empty;
		m_sprite.UseCustomize = true;
		m_sprite.CustomizeTexture = Resources.Load(m_path) as Texture;
		if (m_sprite.CustomizeTexture == null)
		{
			Debug.Log("lose texture!");
		}
		else
		{
			m_sprite.CustomizeRect = new Rect(0f, 0f, 200f, 128f);
		}
	}
}
