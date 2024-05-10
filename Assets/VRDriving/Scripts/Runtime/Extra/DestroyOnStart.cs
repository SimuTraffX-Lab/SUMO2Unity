using UnityEngine;

namespace VRDriving.Extra
{
	/// <summary>
	/// This component forces it's gameObject to self-destruct [Destroy(gameObject)] on Start().
	/// </summary>
	/// Author: Intuitive Gaming Solutions
	public class DestroyOnStart : MonoBehaviour
	{
		// Unity callback(s).
		void Start()
		{
			Destroy(gameObject);
		}
	}
}
