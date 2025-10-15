using System.Threading.Channels;
using Tools;

namespace Days.Y2019.IntCode;

class ProcessingUnit
{
    private Registers<long> Registers;
    private Memory<long> Memory;
    private Decoder<long> Decoder;

    public ChannelWriter<long> IN { get; }
    public ChannelReader<long> OUT { get; }

    public ProcessingUnit(Memory<long> memory)
    {
        var channel = Channel.CreateUnbounded<long>();
        var outchannel = Channel.CreateUnbounded<long>();
        var registers = new Registers<long>
        {
            IN = channel,
            OUT = outchannel,
        };
        Memory = memory;
        Decoder = new Decoder<long>();
        Registers = registers;
        IN = channel;
        OUT = outchannel;
    }

    public async Task Run()
    {
        await Task.Yield();
        var cells = new MemCell<long>[3];

        for (; !Registers.Halt;)
        {
            var ip = Registers.IP;

            var instruction = Decoder.Decode(Memory[Registers.IP++]);

            for (int i = 0; i < instruction.Modes.Length; i++)
            {
                cells[i] = instruction.Modes[i].Get(Memory, Registers, Registers.IP++);
            }
            if (instruction.Op is not null)
                instruction.Op(Registers, cells);
            else
                await instruction.AsycOp(Registers, cells);
        }

        Registers.OUT.Complete();
    }
}