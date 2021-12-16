namespace AoC2021;

static class Extensions
{
    public static int IndexOf<T>(this T[] array, T value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (Equals(array[i], value))
                return i;
        }

        return -1;
    }

    /// <summary>
    /// Assumes that each line is composed of the same amount of characters and returns a grid with all lines.
    /// </summary>
    public static Grid<char> AsGridOfChars(this IEnumerable<string> lines, char? outOfBoundsValue = default) => new(lines, outOfBoundsValue);

    /// <summary>
    /// Assumes that each line is composed of characters '0' to '9', convert them to an array of bytes and returns a grid with all lines.
    /// </summary>
    public static Grid<byte> AsGridOfBytes(this IEnumerable<string> lines, byte? outOfBoundsValue = default) => AsGrid(lines, l => l.Select(c => (byte)(c - '0')), outOfBoundsValue);

    /// <summary>
    /// Converts all lines to a grid, assuming the with of the grid will be based on the number of values returned by the first line.
    /// </summary>
    /// <param name="lines">Lines to convert to grid.</param>
    /// <param name="transform">Function to use to transform each line of text into a collection of values.</param>
    /// <param name="outOfBoundsValue">
    /// A specific value to return when out of bounds coordinates are used when calling <see cref="Grid{T}.Item(long,long)"/>.
    /// Use <see langword="null" /> to raise an <see cref="ArgumentOutOfRangeException"/> when and out of bounds index is used.
    /// </param>
    public static Grid<T> AsGrid<T>(this IEnumerable<string> lines, Func<string, IEnumerable<T>> transform, T? outOfBoundsValue = default) where T : struct => new(lines.Select(l => transform(l)), outOfBoundsValue);

    public static IEnumerable<char> AsChars(this string @this) => @this;
}
