using System;
using System.Collections.Generic;

public enum RoleName
{
	Warrior,
	Children,
	Breeder,
}

public class Role
{
	public Const.Side Side { get; private set; }
	public int RequiredLevel { get; private set; }
	public uint WantedCount { get; private set; }
	public uint EntryCount { get; private set; }

	public Role(Const.Side side, int level, uint wanted)
	{
		Side = side;
		RequiredLevel = level;
		WantedCount = wanted;
		EntryCount = 0;
	}

	public void Update(uint count)
	{
		WantedCount = count;
	}

	public bool Entry(Character c)
	{
		if (c.side != Side || c.level < RequiredLevel)
			return false;

		EntryCount++;

		return true;
	}
	public void Retire(Character c)
	{
		if (EntryCount > 0)
			EntryCount--;
	}
}

public class RoleBlackboard : Blackboard
{
	public Dictionary<RoleName, Role> Roles { get; protected set; }

	public RoleBlackboard()
	{
		Roles = new Dictionary<RoleName, Role>();
	}
}
