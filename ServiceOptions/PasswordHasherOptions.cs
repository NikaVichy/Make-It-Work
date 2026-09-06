namespace MakeItWork.ServiceOptions
{
    public class PasswordHasherOptions
    {
        public byte SaltSize { get; set; }
        public byte HashSize { get; set; }
        public byte Parallelism { get; set; }
        public int MemorySizeKb { get; set; }
        public byte Iterations { get; set; }
        public int Version { get; set; }
    }
}
