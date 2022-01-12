using System.Collections.Generic;

namespace vm
{

	public class Tick
	{
		protected static List<Watcher> temp = new List<Watcher>();
		public static List<Watcher> queue = new List<Watcher>();
		public static IIdMap queueMap = new IdMap();

		public static void add(Watcher w)
		{
			if (!Tick.queueMap.has(w.id))
			{
				Tick.queueMap.add(w.id);
				Tick.queue.Add(w);
			}
		}

		public static void next()
		{
			Tick.queueMap.clear();
			var temp = Tick.queue;
			Tick.queue = Tick.temp;
			Tick.temp = temp;

			foreach (var w in temp)
			{
				w.run();
			}

			temp.Clear();
		}

	}


}