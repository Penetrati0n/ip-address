namespace HyperLogLog
{
    public interface IBytesConverter
    {
        byte[] GetBytes(object obj);
    }
}
