namespace AoC2021;

/// <summary>
/// Represents a point in a space where X move from left to right and Y move from top to bottom.
/// </summary>
readonly struct Point
{
    public static readonly Point Empty = new(0, 0);

    public Point(in long x, in long y)
    {
        X = x;
        Y = y;
    }

    /// <summary>Horizontal coordinate from left to right.</summary>
    public long X { get; }

    /// <summary>Vertical coordinate from top to bottom.</summary>
    public long Y { get; }

    public Point Left => new(X - 1, Y);
    public Point Right => new(X + 1, Y);
    public Point Up => new(X, Y - 1);
    public Point Down => new(X, Y + 1);
    public Point RightDown => new(X + 1, Y + 1);
    public Point RightUp => new(X + 1, Y - 1);
    public Point LeftDown => new(X - 1, Y + 1);
    public Point LeftUp => new(X - 1, Y - 1);

    public static Point operator +(in Point p1, in Point p2) => p1.Add(p2);
    public static Point operator -(in Point p1, in Point p2) => p1.Subtract(p2);
    public static bool operator ==(in Point p1, in Point p2) => p1.X == p2.X && p1.Y == p2.Y;
    public static bool operator !=(in Point p1, in Point p2) => p1.X != p2.X || p1.Y != p2.Y;

    public Point Add(in Point other) => new(X + other.X, Y + other.Y);
    public Point Subtract(in Point other) => new(X - other.X, Y - other.Y);

    public void Deconstruct(out long x, out long y)
    {
        x = X;
        y = Y;
    }

    public IEnumerable<Point> Neighbors()
    {
        yield return Up;
        yield return Left;
        yield return Down;
        yield return Right;
    }

    public IEnumerable<Point> Neighbors(IGrid grid)
    {
        if (grid.Contains(Up))
            yield return Up;
        if (grid.Contains(Left))
            yield return Left;
        if (grid.Contains(Down))
            yield return Down;
        if (grid.Contains(Right))
            yield return Right;
    }


    public override string ToString() => $"({X},{Y})";

    public override bool Equals(object? obj)
    {
        if (obj is Point p2) return this == p2;
        return base.Equals(obj);
    }

    public override int GetHashCode() => HashCode.Combine(X, Y);
}
