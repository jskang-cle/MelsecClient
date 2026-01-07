namespace MelsecClient;

public abstract class MelsecProtocol
{
    private int _receiveTimeout = 1000;
    private int _sendTimeout = 1000;

    public ushort LastError { get; protected set; }

    public int ReceiveTimeout
    {
        get => _receiveTimeout;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(ReceiveTimeout));
            _receiveTimeout = value;
        }
    }

    public int SendTimeout
    {
        get => _sendTimeout;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value, nameof(SendTimeout));
            _sendTimeout = value;
        }
    }

    protected byte[] GetPointBytes(ushort point) => GetBytes(point, 3);

    protected byte[] GetPointCount(int count) => GetBytes(count, 2);

    protected byte[] GetRequestDataLength(int val) => GetBytes(val, 2);

    protected static byte[] GetBytes(int val, byte cnt)
    {
        byte[] tmp = BitConverter.GetBytes(val);
        if (tmp.Length < cnt)
        {
            throw new ArgumentException("Array size mismatch", nameof(val));
        }

        byte[] ret = new byte[cnt];
        Array.Copy(tmp, ret, cnt);
        return ret;
    }

    protected static T[] Concat<T>(T[] array1, T[] array2)
    {
        T[] ret = new T[array1.Length + array2.Length];
        array1.CopyTo(ret, 0);
        array2.CopyTo(ret, array1.Length);
        return ret;
    }

    protected abstract byte[] SendBuffer(byte[] buffer);

    public abstract float ReadReal(ushort point, MelsecDeviceType DeviceType);

    public abstract float[] ReadReal(ushort point, MelsecDeviceType DeviceType, byte count);

    public abstract float[] ReadReal(ushort[] point, MelsecDeviceType DeviceType);

    public abstract void WriteReal(ushort point, float val, MelsecDeviceType DeviceType);

    public abstract void WriteReal(ushort point, float[] val, MelsecDeviceType DeviceType);

    public abstract void WriteReal(ushort[] point, float[] val, MelsecDeviceType DeviceType);

    public abstract uint ReadDword(ushort point, MelsecDeviceType DeviceType);

    public abstract uint[] ReadDword(ushort point, MelsecDeviceType DeviceType, byte count);

    public abstract uint[] ReadDword(ushort[] point, MelsecDeviceType DeviceType);

    public abstract void WriteDword(ushort point, uint val, MelsecDeviceType DeviceType);

    public abstract void WriteDword(ushort point, uint[] val, MelsecDeviceType DeviceType);

    public abstract void WriteDword(ushort[] point, uint[] val, MelsecDeviceType DeviceType);

    public abstract ushort ReadWord(ushort point, MelsecDeviceType DeviceType);

    public abstract ushort[] ReadWord(ushort point, MelsecDeviceType DeviceType, byte count);

    public abstract ushort[] ReadWord(ushort[] point, MelsecDeviceType DeviceType);

    public abstract void WriteWord(ushort point, ushort val, MelsecDeviceType DeviceType);

    public abstract void WriteWord(ushort point, ushort[] val, MelsecDeviceType DeviceType);

    public abstract void WriteWord(ushort[] point, ushort[] val, MelsecDeviceType DeviceType);

    public abstract bool ReadByte(ushort point, MelsecDeviceType DeviceType);

    public abstract bool[] ReadByte(ushort point, MelsecDeviceType DeviceType, byte count);

    public abstract bool[] ReadByte(ushort[] point, MelsecDeviceType DeviceType);

    public abstract void WriteByte(ushort point, bool state, MelsecDeviceType DeviceType);

    public abstract void WriteByte(ushort point, bool[] state, MelsecDeviceType DeviceType);

    public abstract void WriteByte(ushort[] point, bool[] state, MelsecDeviceType DeviceType);

    public abstract void Run(bool forced, ClearMode mode);

    public abstract void Pause(bool forced);

    public abstract void Stop();

    public abstract void Reset();

    public abstract void LatchClear();

    public abstract string ReadCPUModelName();

    public abstract T[] ReadIntelliBuffer<T>(ushort module, int headAddress, int address, byte count) where T : IConvertible;

    public abstract void WriteIntelliBuffer<T>(ushort module, int headAddress, int address, T[] val) where T : IConvertible;

    public abstract T[] ReadBuffer<T>(int address, byte count) where T : IConvertible;

    public abstract void WriteBuffer<T>(int address, T[] val) where T : IConvertible;

    public abstract T[] BatchReadWord<T>(ushort point, MelsecDeviceType DeviceType, ushort count) where T : IConvertible;

    public abstract void BatchWriteWord<T>(ushort point, T[] val, MelsecDeviceType DeviceType) where T : IConvertible;

    public abstract T[] RandomReadWord<T>(ushort[] point, MelsecDeviceType DeviceType) where T : IConvertible;

    public abstract void RandomWriteWord<T>(ushort[] point, T[] val, MelsecDeviceType DeviceType) where T : IConvertible;
}
