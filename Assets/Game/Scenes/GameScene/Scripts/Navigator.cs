using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Navigator
{
	public class Node
	{
		public Vector3 position;
		public Node prev;
		public float stacked_cost;
		public float predictive_cost;
		public float cost
		{
			get { return stacked_cost + predictive_cost; }
		}
	}

	public int LoopLimit { get; set; }

	Const.MapCellType[,] map;

	public long GetId(Node node)
	{
		return (int)node.position.x + map.GetLength(0) * (int)node.position.y;
	}

	public Navigator(Const.MapCellType[,] map)
	{
		this.map = map;
		LoopLimit = 30;
	}

	public Vector3[] FindPath(Vector3 origin, Vector3 dest)
	{
		var open = new Dictionary<long, Node>();
		var closed = new Dictionary<long, Node>();

		var dest_node = new Node() {
			position = dest,
		};

		var start = new Node() {
			position = origin,
			stacked_cost = 0,
		};
		start.predictive_cost = PredictCost(start, dest_node);
		open.Add(GetId(start), start);

		Predicate<Node> is_dest = (n) => (GetId(n) == GetId(dest_node));

		for (int i = 0; i < LoopLimit; i++)
		{
			Debug.Log("open " + open.Count() + "/" + closed.Count());
			if (open.Count() == 0)
				return null;

			var pair = open.Aggregate((a, b) => (a.Value.cost <= b.Value.cost ? a : b));
			var node = pair.Value;
			Debug.Log("node " + (int)node.position.x + " " + (int)node.position.y);
			for (int j = 0; j < 4; j++)
			{
				Vector3 pos = node.position;
				if (j == 0 || j == 2)
					pos.x += j - 1;
				if (j == 1 || j == 3)
					pos.y += j - 2;
				var next = new Node() {
					position = pos,
					prev = node,
					stacked_cost = node.stacked_cost + 1,
				};
				var id = GetId(next);
				if (open.ContainsKey(id) || closed.ContainsKey(id))
					continue;
				if (!IsWalkable(next))
					continue;
				if (is_dest(next))
					return TracePath(next);

				next.predictive_cost = PredictCost(next, dest_node);
				open.Add(id, next);
			}
			open.Remove(pair.Key);
			closed.Add(GetId(node), node);
		}

		return null;
	}

	private Vector3[] TracePath(Node tail)
	{
		var path = new List<Vector3>();

		for (var node = tail; node.prev != null; node = node.prev)
			path.Add(node.position - node.prev.position);

		path.Reverse();
		return path.ToArray();
	}

	private bool IsWalkable(Node point)
	{
		var x = (int)point.position.x;
		var y = (int)point.position.y;
		if (x < 0 || y < 0 || x >= map.GetLength(0) || y >= map.GetLength(1))
			return false;
		return map[x, y] == Const.MapCellType.None;
	}

	private float PredictCost(Node current, Node dest)
	{
		return Math.Abs(current.position.x - dest.position.x) + Math.Abs(current.position.y - dest.position.y);
	}
}
