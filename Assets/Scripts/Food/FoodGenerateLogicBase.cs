using UnityEngine;

public abstract class FoodGenerateLogicBase : MonoBehaviour
{
	/// <summary>
	/// マップ生成後に呼ばれる
	/// </summary>
	public abstract void OnInit();
}
