using System.Globalization;

namespace MelsecClient;

public abstract class MelsecClient
{
    protected readonly MelsecProtocol melsecProtocol;

    protected MelsecClient(ProtocolType protocoltype, string ip, ushort port, int receiveTimeout, int sendTimeout)
    {
        melsecProtocol = protocoltype switch
        {
            ProtocolType.Melsec3EProtocol => new Melsec3EProtocol(ip, port),
            ProtocolType.Melsec4EProtocol => new Melsec4EProtocol(ip, port),
            _ => throw new ArgumentOutOfRangeException(nameof(protocoltype), protocoltype, "Invalid protocol type")
        };
        melsecProtocol.SendTimeout = sendTimeout;
        melsecProtocol.ReceiveTimeout = receiveTimeout;
    }

    public bool SetQTime(DateTime datetime)
    {
        string sd210 = datetime.ToString("yyMM");
        ushort id210 = ushort.Parse(sd210, NumberStyles.HexNumber);
        string sd211 = datetime.ToString("ddHH");
        ushort id211 = ushort.Parse(sd211, NumberStyles.HexNumber);
        string sd212 = datetime.ToString("mmss");
        ushort id212 = ushort.Parse(sd212, NumberStyles.HexNumber);
        string sd213 = datetime.ToString("yyyy");
        sd213 = sd213[..2] + "0" + datetime.DayOfWeek.ToString("d");
        ushort id213 = ushort.Parse(sd213, NumberStyles.HexNumber);
        int d210 = (id211 << 16) | id210;
        int d212 = (id213 << 16) | id212;
        melsecProtocol.WriteByte([213, 211], [false, false], MelsecDeviceType.SpecialRelay);
        melsecProtocol.WriteDword([210, 212], [(uint)d210, (uint)d212], MelsecDeviceType.SpecialRegister);
        melsecProtocol.WriteByte(210, false, MelsecDeviceType.SpecialRelay);
        melsecProtocol.WriteByte(210, true, MelsecDeviceType.SpecialRelay);
        melsecProtocol.WriteByte(210, false, MelsecDeviceType.SpecialRelay);
        return !melsecProtocol.ReadByte(211, MelsecDeviceType.SpecialRelay);
    }

    private void BcdToInt(ref uint bcd)
    {
        bcd = uint.Parse(bcd.ToString("x"));
    }

    public DateTime GetQTime()
    {
        melsecProtocol.WriteByte(213, true, MelsecDeviceType.SpecialRelay);
        uint[] d = melsecProtocol.ReadDword(210, MelsecDeviceType.SpecialRegister, 2);
        uint d210 = d[0];
        uint d212 = d[1];
        melsecProtocol.WriteByte(213, false, MelsecDeviceType.SpecialRelay);
        uint Year = (((d212 >> 16) & 0xFF00) | ((d210 >> 8) & 0xFF));
        uint Month = (d210 & 0xFF);
        uint Day = ((d210 >> 24) & 0xFF);
        uint Hour = ((d210 >> 16) & 0xFF);
        uint Minute = ((d212 >> 8) & 0xFF);
        uint Second = (d212 & 0xFF);
        //uint DayOfWeek = ((id212 >> 16) & 0xFF);
        BcdToInt(ref Year);
        BcdToInt(ref Month);
        BcdToInt(ref Day);
        BcdToInt(ref Hour);
        BcdToInt(ref Minute);
        BcdToInt(ref Second);
        //HexInt(ref DayOfWeek);
        return new DateTime((int)Year, (int)Month, (int)Day, (int)Hour, (int)Minute, (int)Second);
    }

    public CpuStatus ReadCPUStatus()
    {
        ushort status = melsecProtocol.ReadWord(203, MelsecDeviceType.SpecialRegister);
        int bStatus = status & ((1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 15));
        return bStatus switch
        {
            0 => CpuStatus.RUN,
            1 => CpuStatus.STEPRUN,
            2 => CpuStatus.STOP,
            3 => CpuStatus.PAUSE,
            _ => CpuStatus.NONE
        };
    }

    public SwitchStatus ReadSwitchStatus()
    {
        ushort status = melsecProtocol.ReadWord(200, MelsecDeviceType.SpecialRegister);
        int bStatus = status & ((1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 15));
        return bStatus switch
        {
            0 => SwitchStatus.RUN,
            1 => SwitchStatus.STOP,
            2 => SwitchStatus.LCLR,
            _ => SwitchStatus.NONE
        };
    }

    public StopPauseCause ReadStopPauseCause()
    {
        ushort status = melsecProtocol.ReadWord(203, MelsecDeviceType.SpecialRegister);
        int bStatus = (status & ((1 << 4) | (1 << 5) | (1 << 6) | (1 << 7) | (1 << 15))) >> 4;
        return bStatus switch
        {
            0 => StopPauseCause.BySwitch,
            1 => StopPauseCause.RemoteRelay,
            2 => StopPauseCause.RemoteDevice,
            3 => StopPauseCause.ByProgram,
            4 => StopPauseCause.ByError,
            _ => StopPauseCause.None
        };
    }
}
