using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Navigator
{
	readonly int LoopLimit = 30;

	class Node
	{
		public int x, y;
		public bool closed;
		public Node prev;
		public float stacked_cost;
		public float predictive_cost;
		public float cost
		{
			get { return stacked_cost + predictive_cost; }
		}
		public override int GetHashCode()
		{
			return x.GetHashCode() ^ y.GetHashCode();
		}
	}

	Const.MapCellType[,] map;

	public Navigator(Const.MapCellType[,] map)
	{
		map = map;
	}

	public Vector3[] FindPath(Vector3 origin, Vector3 dest)
	{
		var candidates = new List<Node>();

		var dest_node = new Node() {
			x = (int) dest.x,
			y = (int) dest.y,
		};

		var start = new Node() {
			x = (int) origin.x,
			y = (int) origin.y,
			closed = false,
			stacked_cost = 0,
		};
		start.predictive_cost = PredictCost(start, dest_node);

		Predicate<Node> is_dest = (n) => (n.x == (int) dest.x && n.y == (int) dest.y);

		for (int i = 0; i < LoopLimit; i++)
		{
			var open = candidates.Where((n) => !n.closed);
			if (open.Count() == 0)
				return null;

			var node = open.Aggregate((a, b) => (a.cost <= b.cost ? a : b));
			for (int j = 0; j < 4; j++)
			{
				var next = new Node() {
					x = node.x + (j == 0 || j == 2 ? j - 1 : 0),
					y = node.y + (j == 1 || j == 3 ? j - 2 : 0),
					closed = false,
					prev = node,
					stacked_cost = node.stacked_cost + 1,
				};
				if (!IsWalkable(next))
					continue;
				if (is_dest(next))
					return TracePath(next);

				next.predictive_cost = PredictCost(next, dest_node);
				candidates.Add(next);
			}
			node.closed = true;
		}

		return null;
	}

	private Vector3[] TracePath(Node tail)
	{
		var path = new List<Vector3>();

		for (var node = tail; node.prev != null; node = node.prev)
			path.Add(new Vector3(node.x - node.prev.x, node.y - node.prev.y));

		return path.ToArray();
	}

	private bool IsWalkable(Node point)
	{
		if (point.x < 0 || point.y < 0 || point.x >= map.GetLength(0) || point.y >= map.GetLength(1))
			return false;
		return map[point.x, point.y] == Const.MapCellType.None;
	}

	private float PredictCost(Node current, Node dest)
	{
		return Math.Abs(current.x - dest.x) + Math.Abs(current.y - dest.y);
	}
}
