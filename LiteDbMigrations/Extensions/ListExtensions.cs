namespace LiteDbMigrations.Extensions
{
    internal static class ListExtensions
    {
        internal static List<List<T>>? SplitList<T>(this IEnumerable<T> values, int groupSize, int? maxCount = null)
        {
            List<List<T>> list = [];
            if (values == null)
            {
                return null;
            }

            List<T> list2 = values.ToList();
            if (list2.Count <= groupSize)
            {
                list.Add(list2);
            }
            else
            {
                int i = 0;
                int num;
                for (int count = list2.Count; i < count && (!maxCount.HasValue || i < maxCount); i += num)
                {
                    num = (i + groupSize > count) ? (count - i) : groupSize;
                    list.Add(list2.GetRange(i, num));
                }
            }

            return list;
        }
    }
}
