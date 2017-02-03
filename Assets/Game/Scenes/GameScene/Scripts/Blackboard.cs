using System;

public abstract class Blackboard
{
	public uint Version { get; protected set; }

	protected void IncrementVersion()
	{
		Version++;
	}
}
