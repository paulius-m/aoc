namespace Days.Y2019.IntCode
{
    public class MemCell
    {
        public int Value;
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class Registers
    {
        public int IP = 0;
        public bool Halt = false;
    }

}
