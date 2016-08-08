using UnityEngine;
using System.Collections;

public class GameFood : MonoBehaviour
{
	public void OnEat()
	{
		Destroy(this.gameObject);
	}
}
