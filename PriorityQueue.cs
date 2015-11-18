using System;
using System.Collections;
using System.Collections.Generic;

namespace PriorityQueue
{
	class PriorityQueue <T> where T : IComparable <T>
	{
		// based on binary heap structure
		private List <T> queue;

		public PriorityQueue()
		{
			queue = new List <T>();
		}

		public void Enqueue(T queueItem)
		{
			queue.Add (queueItem);

			int childIndex = queue.Count - 1;

			while (childIndex > 0) 
			{
				int parentIndex = (childIndex - 1) / 2;

				if (queue[childIndex].CompareTo(queue[parentIndex]) >= 0)
				{
					break;
				}

				SwitchPosition(childIndex, parentIndex);

				childIndex = parentIndex;
			}
		}

		public T Dequeue()
		{
			if (queue.Count == 0) { return default(T); }

			T frontItem = PopFrontItem ();

			if (queue.Count > 0) { SortQueue (); }

			return frontItem;
		}

		public T Peek()
		{
			T frontItem = queue [0];
			return frontItem;
		}

		private T PopFrontItem()
		{
			int lastIndex = queue.Count - 1;

			T frontItem = queue [0];
			queue [0] = queue [lastIndex];
			queue.RemoveAt (lastIndex);

			return frontItem;
		}

		private void SortQueue()
		{
			int lastIndex = queue.Count - 1;
			int parentIndex = 0;

			while (true) 
			{
				int childIndex = parentIndex * 2 + 1;

				if (childIndex > lastIndex) { break; }

				int nextChildIndex = childIndex + 1;

				if (nextChildIndex <= lastIndex && queue [nextChildIndex].CompareTo (queue [childIndex]) < 0 )
				{
					childIndex = nextChildIndex;
				}

				if (queue [parentIndex].CompareTo (queue [childIndex]) <= 0 ) { break; }

				SwitchPosition(parentIndex, childIndex);

				parentIndex = childIndex;
			}
		}

		private void SwitchPosition(int ii, int jj)
		{
			T tmp = queue[ii];
			queue[ii] = queue[jj];
			queue[jj] = tmp;
		}
	}
}
