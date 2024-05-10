using UnityEngine;

namespace VRDriving.Extra
{
	/// <summary>
	/// This component forces it's gameObject to become inactive on OnEnable().
	/// </summary>
	/// Author: Intuitive Gaming Solutions
	public class HideOnEnable : MonoBehaviour
	{
		// Unity callback(s).
		void OnEnable()
		{
			gameObject.SetActive(false);
		}
	}
}
