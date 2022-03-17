using System;
using System.Net;
using AsmJitter;
using AsmJitter.Model;
using AsmJitter.Model.Instruction;
using AsmJitter.Model.Operand;

namespace FIFAConnectorClient;

internal static class GameManager
{        

    private static IntPtr _handle;

    internal static void Init(IPAddress targetIp)
    {
        _handle = MemoryManager.GetProcHandle();
        if (_handle == IntPtr.Zero)
        {
            throw new Exception("Could not find the game process, please start the game at first...");
        }
        LoadAlternativeTargetIp(targetIp);
    }

    private static void LoadAlternativeTargetIp(IPAddress targetIp)
    {
        var changeIpCode = GetChangeDestinationIpAddressCode(targetIp);
        var codeBytes = changeIpCode.GetBytes();
        var injectAddress = MemoryManager.AllocSpace(_handle, (uint)codeBytes.Count).ToInt32();
        MemoryManager.WriteByteArrayFromMemory(_handle, injectAddress, codeBytes.ToArray());

        var callCode = new Code();
        callCode.Call(new FourBytesConst(injectAddress))
            .Nop()
            .Nop();

        var callBytes = callCode.GetBytes(Constants.InstructionPushEaxDestIp);
        MemoryManager.WriteByteArrayFromMemory(_handle, Constants.InstructionPushEaxDestIp, callBytes.ToArray());
    }
    
    private static Code GetChangeDestinationIpAddressCode(IPAddress targetIp)
    {
        var ipAddressBytes = targetIp.GetAddressBytes();
        var firstPart = GetConstValue(ipAddressBytes[0]);
        var secondPart = GetConstValue(ipAddressBytes[1]);
        var thirdPart = GetConstValue(ipAddressBytes[2]);
        var fourthPart = GetConstValue(ipAddressBytes[3]);

        var shellcode = new Code();
        shellcode.Mov(new Register(RegisterEnum.ECX_XMM1), new RegisterMemory(RegisterEnum.ESP_SIB_XMM4, 0x3C))
            .Mov(new Register(RegisterEnum.EDX_XMM2), new RegisterMemory(RegisterEnum.EBP_DS32_XMM5, 0x18))
            .Mov(new RegisterMemory(RegisterEnum.EAX_XMM0, 0x04), firstPart)
            .Mov(new RegisterMemory(RegisterEnum.EAX_XMM0, 0x05), secondPart)
            .Mov(new RegisterMemory(RegisterEnum.EAX_XMM0, 0x06), thirdPart)
            .Mov(new RegisterMemory(RegisterEnum.EAX_XMM0, 0x07), fourthPart)
            .Ret();
        return shellcode;
    }

    private static AbstractConst GetConstValue(byte value)
    {
        if (value > 0x7F)
        {
            return new FourBytesConst(value);
        }

        return new EightBitConstant(Convert.ToSByte(value));
    }

}
