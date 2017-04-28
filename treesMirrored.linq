<Query Kind="Program" />

void Main()
{
//	var test = new Node<int>
//	{
//		Left = new Node<int>(),
//		Right = new Node<int> { Left = new Node<int>() }
//	};

	var test = new Node<int>
	{
		Left = new Node<int> { Right = new Node<int>() },
		Right = new Node<int> { Left = new Node<int>() }
	};
	
	test.IsSymmetric().Dump();
}

// Define other methods and classes here
public class Node<T> { public Node<T> Left {get;set;} public Node<T> Right {get;set;} }

public static class NodeExt
{
	public static bool IsSymmetric<T>(this Node<T> node)
	{
		if (node == null) return false;

		return node.Left.Descendants().Mirrors(node.Right.Descendants());
	}

	public static bool Mirrors<T>(this IEnumerable<Node<T>> left, IEnumerable<Node<T>> right)
	{
		return left.ZipLongest(right, (l,r) => new {left = l, right = r})
				   .All(pair => IsMirrored(pair.left, pair.right));
	}

	private static bool IsMirrored<T>(Node<T> left, Node<T> right) 
	{
		return (left == null && right == null
			|| left != null && right != null);
	}

	public static IEnumerable<Node<T>> Descendants<T>(this Node<T> node)
	{
		if (node == null) yield break;

		var nodes = new Queue<Node<T>>();
		nodes.Enqueue(node);

		while (nodes.Any())
		{
			Node<T> current = nodes.Dequeue();
			if (current.Left != null)
			{
				nodes.Enqueue(node.Left);
			}
			if (current.Right != null)
			{
				nodes.Enqueue(node.Right);
			}
			yield return current;
		}
	}
	
	
	public static IEnumerable<TResult> ZipLongest<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first,
             IEnumerable<TSecond> second,
             Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            return ZipLongestImpl(first, second, resultSelector);
        }

        static IEnumerable<TResult> ZipLongestImpl<TFirst, TSecond, TResult>(
            IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TSecond, TResult> resultSelector)
        {
            using (var e1 = first.GetEnumerator())
            using (var e2 = second.GetEnumerator())
            {
                while (e1.MoveNext())
                {
                    if (e2.MoveNext())
                    {
                        yield return resultSelector(e1.Current, e2.Current);
                    }
                    else
                    {
                        do { yield return resultSelector(e1.Current, default(TSecond)); }
                        while (e1.MoveNext());
                        yield break;
                    }
                }
                if (e2.MoveNext())
                {
                    do { yield return resultSelector(default(TFirst), e2.Current); }
                    while (e2.MoveNext());
                }
            }
        }
}