using UnityEngine;
using com.tksr.statemachine.defines;

public class Equippable : MonoBehaviour 
{
	/// <summary>
	/// The EquipSlots flag which is the default
	/// equip location(s) for this item.
	/// For example, a normal weapon would only specify 
	/// primary, but a two-handed weapon would specify
	/// both primary and secondary.
	/// </summary>
	public EnumEquipSlots defaultSlots;

	/// <summary>
	/// Some equipment may be allowed to be equipped in
	/// more than one slot location, such as when 
	/// dual-wielding swords.
	/// </summary>
	public EnumEquipSlots secondarySlots;

	/// <summary>
	/// The slot(s) where an item is currently equipped
	/// </summary>
	public EnumEquipSlots slots;

	bool _isEquipped;

	public void OnEquip()
	{
		if (_isEquipped)
			return;

		_isEquipped = true;

		Feature[] features = GetComponentsInChildren<Feature>();
		for (int i = 0; i < features.Length; ++i)
			features[i].Activate(gameObject);
	}

	public void OnUnEquip()
	{
		if (!_isEquipped)
			return;

		_isEquipped = false;

		Feature[] features = GetComponentsInChildren<Feature>();
		for (int i = 0; i < features.Length; ++i)
			features[i].Deactivate();
	}
}