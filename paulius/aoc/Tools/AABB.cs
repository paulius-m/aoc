namespace Tools;

public record AABB
{
    public CoordRange[] Ranges { get; set; }

    public AABB[] Partition(CoordRange r, int dim)
    {
        var c = Ranges[dim];

        if (!c.Overlaps(r))
        {
            return [this];
        }

        var cuts = new CoordRange[]
        {
            new(Math.Min(r.From, c.From), Math.Max(r.From, c.From) - 1),
            new(Math.Max(r.From, c.From), Math.Min(r.To, c.To)),
            new(Math.Min(r.To, c.To) + 1, Math.Max(r.To, c.To))
        }
        .Where(r => r.From <= r.To)
        .Where(c.Contains).ToArray();

        return cuts.Select(c =>
        {
            var copy = Ranges.ToArray();
            copy[dim] = c;
            return new AABB { Ranges = copy };
        }).ToArray();
    }

    public AABB[] Partition(AABB c)
    {
        var partitions = new List<AABB> { this };
        for (int i = 0; i < c.Ranges.Length; i++)
        {
            var newPartitions = new List<AABB>();

            foreach (var p in partitions)
            {
                if (p.Overlaps(c))
                {
                    newPartitions.AddRange(p.Partition(c.Ranges[i], i));
                }
                else
                {
                    newPartitions.Add(p);
                }
            }

            partitions = newPartitions;
        }

        return partitions.ToArray();
    }

    public bool Contains(AABB c)
    {
        return Ranges.Zip(c.Ranges, (s, r) => s.Contains(r)).All(Operators.Identity);
    }

    public bool Overlaps(AABB c)
    {
        return Ranges.Zip(c.Ranges, (s, r) => s.Overlaps(r)).All(Operators.Identity);
    }


    public bool Overlaps2(AABB c)
    {
        var r = Math.Min(Ranges.Length, c.Ranges.Length);
        for (int i = 0; i < r; i++)
        {
            if (!Ranges[i].Overlaps(c.Ranges[i]))
            {
                return false;
            }
        }

        return true;
    }

    public bool Contains(float[] point)
    {
        return Ranges.Zip(point, (r, p) => r.Contains(p)).All(Operators.Identity);
    }

    public long Volume()
    {
        var v = Ranges.Product(cr => cr.To - cr.From + 1);
        return v;
    }
}
