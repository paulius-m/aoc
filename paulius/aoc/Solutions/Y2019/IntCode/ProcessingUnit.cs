using System.Threading.Channels;

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
        Decoder = new Decoder<long>(memory, registers);
        Registers = registers;
        IN = channel;
        OUT = outchannel;
    }

    public async Task Run()
    {
        await Task.Yield();
        for (; !Registers.Halt;)
        {
            var instruction = Decoder.Decode(Memory[Registers.IP++]);

            var cells = instruction.Modes.Select(m => m[Registers.IP++]).ToArray();
            //Console.WriteLine(instruction.Exec.Method);
            await instruction.Exec(Registers, cells);
        }
        Registers.OUT.Complete();
    }
}