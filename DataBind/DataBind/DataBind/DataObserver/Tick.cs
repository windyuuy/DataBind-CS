using System.Collections.Generic;

namespace DataBind.VM
{

	public class Tick
	{
		protected static List<Watcher> Temp = new List<Watcher>();
		public static List<Watcher> Queue = new List<Watcher>();
		public static IIdMap QueueMap = new IdMap();

		public static void Add(Watcher w)
		{
			if (!Tick.QueueMap.Has(w.id))
			{
				Tick.QueueMap.Add(w.id);
				Tick.Queue.Add(w);
			}
		}

		public static void Next()
		{
			Tick.QueueMap.Clear();
			var temp = Tick.Queue;
			Tick.Queue = Tick.Temp;
			Tick.Temp = temp;

			foreach (var w in temp.ToArray())
			{
				w.run();
			}

			temp.Clear();
		}

	}


}