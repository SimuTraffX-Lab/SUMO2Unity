using UnityEngine;

namespace VRDriving.Extra
{
	/// <summary>
	/// This component forces it's gameObject to become inactive on Start().
	/// </summary>
	/// Author: Intuitive Gaming Solutions
	public class HideOnStart : MonoBehaviour
	{
		// Unity callback(s).
		void Start()
		{
			gameObject.SetActive(false);
		}
	}
}
