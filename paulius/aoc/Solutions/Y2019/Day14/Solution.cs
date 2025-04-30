using Sprache;
using Tools;
using Input = System.Linq.ILookup<string, (Days.Y2019.Day14.Substance r, Days.Y2019.Day14.Substance p)>;

namespace Days.Y2019.Day14;

file class Solution : ISolution<Input>
{
    public async Task<Input> LoadInput()
    {
        var input = await File.ReadAllTextAsync(this.GetInputFile("input"));

        var reactions = (from r in ReactionParser.DelimitedBy(Parse.String(Environment.NewLine).Text())
                         select r.ToArray()).Parse(input).Append((r: new[] { new Substance(1, "ORE") }, p: new Substance(1, "ORE")));

        var reactionsGrapth = reactions.SelectMany(r => r.r.Select(r1 => (r: r1, r.p))).ToLookup(r => r.p.Name);

        return reactionsGrapth;
    }

    public object Part1(Input reactionsGrapth)
    {
        var demand = new Dictionary<string, long>();
        demand["FUEL"] = 1;
        var supply = new Dictionary<string, long>();
        do
        {
            demand = CalculateDemand(reactionsGrapth, demand, supply);
        } while (!demand.All(kv => kv.Key == "ORE"));

        return demand["ORE"];
    }

    private static Dictionary<string, long> CalculateDemand(Input reactionsGrapth, Dictionary<string, long> demand, Dictionary<string, long> supply)
    {
        var newDemand = new Dictionary<string, long>();

        foreach (var (substance, amount) in demand)
        {
            var supplyAmount = supply.GetValueOrDefault(substance, 0);

            var (_, product) = reactionsGrapth[substance].First();

            var demandAmount = amount - supplyAmount;

            var multiply = (int)Math.Ceiling(demandAmount * 1.0 / product.Amount);

            foreach (var (reactant, _) in reactionsGrapth[substance])
            {
                newDemand[reactant.Name] = newDemand.GetValueOrDefault(reactant.Name, 0) + multiply * reactant.Amount;
            }
            supply[substance] = product.Amount * multiply - demandAmount;
        }
        demand = newDemand;
        return demand;
    }

    public object Part2(Input reactionsGrapth)
    {
        var demand = new Dictionary<string, long>();
        var supply = new Dictionary<string, long>();
        supply["ORE"] = 1000000000000;
        var fuelDone = 0;
        var inc = 1000;
        do
        {
            if (supply["ORE"] < inc * demand.GetValueOrDefault("ORE", 0))
            {
                inc = 1;
            }

            demand["FUEL"] = inc;
            do
            {
                demand = CalculateDemand(reactionsGrapth, demand, supply);
            } while (!demand.All(kv => kv.Key == "ORE"));
            fuelDone += inc;
        } while (supply["ORE"] >= demand.GetValueOrDefault("ORE", 0));

        return fuelDone - 1;
    }

    static Parser<Substance> SubstanceParser = from amount in Parse.Number.Select(int.Parse).Token()
                                               from name in Parse.Letter.Many().Text()
                                               select new Substance(amount, name);

    static Parser<(Substance[] r, Substance p)> ReactionParser = from reactants in SubstanceParser.DelimitedBy(Parse.Char(',')).Select(Enumerable.ToArray)
                                                                 from reactionArrow in Parse.String("=>").Token()
                                                                 from product in SubstanceParser
                                                                 select (reactants, product);
}

class Substance
{
    public int Amount;
    public string Name;

    public Substance(int amount, string name)
    {
        Amount = amount;
        Name = name;
    }
    public override string ToString() => $"{Amount} {Name}";
}