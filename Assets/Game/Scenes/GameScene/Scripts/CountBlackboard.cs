using System;
using System.Collections.Generic;

public class CountBlackboard : Blackboard
{
	public Dictionary<int, uint> OwnLevelDistribution { get; protected set; }
	public Dictionary<int, uint> OppLevelDistibution { get; protected set; }

	public CountBlackboard()
	{
		OppLevelDistibution = new Dictionary<int, uint>();
		OwnLevelDistribution = new Dictionary<int, uint>();

		foreach (var level in Setting.characterLevels)
		{
			OppLevelDistibution.Add(level.level, 0);
			OwnLevelDistribution.Add(level.level, 0);
		}
	}

	public void Add(Character c)
	{
		switch (c.side)
		{
		case Const.Side.Own:
			OwnLevelDistribution[c.level]++;
			break;
		case Const.Side.Opp:
			OppLevelDistibution[c.level]++;
			break;
		}

		IncrementVersion();
	}
	public void Remove(Character c)
	{
		switch (c.side)
		{
		case Const.Side.Own:
			OwnLevelDistribution[c.level]--;
			break;
		case Const.Side.Opp:
			OppLevelDistibution[c.level]--;
			break;
		}

		IncrementVersion();
	}
	public void Update(Character c, int nextLevel)
	{
		switch (c.side)
		{
		case Const.Side.Own:
			OwnLevelDistribution[c.level]--;
			OwnLevelDistribution[nextLevel]++;
			break;
		case Const.Side.Opp:
			OppLevelDistibution[c.level]--;
			OppLevelDistibution[nextLevel]++;
			break;
		}

		IncrementVersion();
	}
}
