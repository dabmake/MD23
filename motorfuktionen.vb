Option Strict Off
Option Explicit Off
Module motorfuktionen
    Public Function Voltage() As Byte
        Dim Ret As Integer
        Dim Report(8) As Byte
        If Form2.IOWarrior <> 0 And (Form2.Pid = IOWKIT_PID_IOW40 Or Form2.Pid = IOWKIT_PID_IOW24) Then

            Report(0) = &H2 ' ReportID IIC write request
            Report(1) = &HC2 ' 2 bytes with IIC Start and Stop
            Report(2) = &HB0S ' MD23 address byte = ADR 0, write
            Report(3) = VOLT_BAT

            Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
            ' swallow ACK report
            Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

            Report(0) = &H3 ' ReportID IIC read request
            Report(1) = &H1 ' 1 Bytes
            Report(2) = &HB1S        ' read address

            Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
            ' read answer containing voltage value
            Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
            ' extract bytes of value
            Voltage = Report(2)

        End If
    End Function
    Public Function Motor_Ticks_Left() As Long
        Dim Ret As Integer
        Dim Report(8) As Byte
        Dim a, b, c, d As Byte
        Report(0) = &H2 ' ReportID IIC write request
        Report(1) = &HC2 ' 1 bytes with IIC Start and Stop
        Report(2) = &HB0S ' MD23 address byte = ADR 0, write
        Report(3) = ENC1A

        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' swallow ACK report
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

        Report(0) = &H3S ' ReportID IIC read request
        Report(1) = &H1S ' 1 Bytes
        Report(2) = &HB1S         ' read address

        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' read answer containing voltage value
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' extract bytes of value
        a = Report(2)

        Report(0) = &H2S ' ReportID IIC write request
        Report(1) = &HC2S ' 1 bytes with IIC Start and Stop
        Report(2) = &HB0S ' MD23 address byte = ADR 0, write
        Report(3) = ENC1B

        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' swallow ACK report
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

        Report(0) = &H3S ' ReportID IIC read request
        Report(1) = &H1S ' 1 Bytes
        Report(2) = &HB1S        ' read address

        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' read answer containing voltage value
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' extract bytes of value
        b = Report(2)

        Report(0) = &H2S ' ReportID IIC write request
        Report(1) = &HC2S ' 1 bytes with IIC Start and Stop
        Report(2) = &HB0S ' MD23 address byte = ADR 0, write
        Report(3) = ENC1C

        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' swallow ACK report
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

        Report(0) = &H3 ' ReportID IIC read request
        Report(1) = &H1 ' 1 Bytes
        Report(2) = &HB1S        ' read address

        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' read answer containing voltage value
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' extract bytes of value
        c = Report(2)

        Report(0) = &H2S ' ReportID IIC write request
        Report(1) = &HC2S ' 1 bytes with IIC Start and Stop
        Report(2) = &HB0S ' MD23 address byte = ADR 0, write
        Report(3) = ENC1D

        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' swallow ACK report
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

        Report(0) = &H3S ' ReportID IIC read request
        Report(1) = &H1S ' 1 Bytes
        Report(2) = &HB1S        ' read address

        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' read answer containing voltage value
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' extract bytes of value
        d = Report(2)


        Motor_Ticks_Left = d + c * 256 + b * 65535
        Motor_Ticks_Left = CInt(Motor_Ticks_Left) + a * CLng(16777216)

        If Motor_Ticks_Left > 2147483648 Then
            Motor_Ticks_Left = Motor_Ticks_Left - CLng(4294967040)
        End If




    End Function
    Public Function Motor_Ticks_Right() As Long
        Dim Ret As Integer
        Dim Report(8) As Byte
        Dim a, b, c, d As Byte
        Report(0) = &H2 ' ReportID IIC write request
        Report(1) = &HC2 ' 1 bytes with IIC Start and Stop
        Report(2) = &HB0S ' MD23 address byte = ADR 0, write
        Report(3) = ENC2A

        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' swallow ACK report
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

        Report(0) = &H3S ' ReportID IIC read request
        Report(1) = &H1S ' 1 Bytes
        Report(2) = &HB1S         ' read address

        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' read answer containing value
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' extract bytes of value
        a = Report(2)

        Report(0) = &H2S ' ReportID IIC write request
        Report(1) = &HC2S ' 1 bytes with IIC Start and Stop
        Report(2) = &HB0S ' MD23 address byte = ADR 0, write
        Report(3) = ENC2B

        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' swallow ACK report
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

        Report(0) = &H3S ' ReportID IIC read request
        Report(1) = &H1S ' 1 Bytes
        Report(2) = &HB1S        ' read address

        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' read answer containing voltage value
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' extract bytes of value
        b = Report(2)

        Report(0) = &H2S ' ReportID IIC write request
        Report(1) = &HC2S ' 1 bytes with IIC Start and Stop
        Report(2) = &HB0S ' MD23 address byte = ADR 0, write
        Report(3) = ENC2C

        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' swallow ACK report
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

        Report(0) = &H3 ' ReportID IIC read request
        Report(1) = &H1 ' 1 Bytes
        Report(2) = &HB1S        ' read address

        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' read answer containing voltage value
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' extract bytes of value
        c = Report(2)

        Report(0) = &H2S ' ReportID IIC write request
        Report(1) = &HC2S ' 1 bytes with IIC Start and Stop
        Report(2) = &HB0S ' MD23 address byte = ADR 0, write
        Report(3) = ENC2D

        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' swallow ACK report
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

        Report(0) = &H3S ' ReportID IIC read request
        Report(1) = &H1S ' 1 Bytes
        Report(2) = &HB1S        ' read address

        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' read answer containing voltage value
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' extract bytes of value
        d = Report(2)


        Motor_Ticks_Right = d + c * 256 + b * 65535
        Motor_Ticks_Right = CInt(Motor_Ticks_Right) + a * CLng(16777216)

        If Motor_Ticks_Right > 2147483648 Then
            Motor_Ticks_Right = Motor_Ticks_Right - CLng(4294967040)
        End If


    End Function
    Public Function Current_left() As Byte
        Dim Ret As Byte
        Dim Report(8) As Byte
        Report(0) = &H2S ' ReportID IIC write request
        Report(1) = &HC2S ' 2 bytes with IIC Start and Stop
        Report(2) = &HB0S ' MD23 address byte = ADR 0, write
        'Report(3) = &HCS
        Report(3) = CURRENT_MOTOR2


        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' swallow ACK report
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)


        'System.Threading.Thread.Sleep(40)

        Report(0) = &H3S ' ReportID IIC read request
        Report(1) = &H2S ' 1 Bytes
        Report(2) = &HB1S        ' read address
        Report(3) = &HCS
        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' read answer containing voltage value
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' extract bytes of value
        Current_left = Report(2)

    End Function
    Public Function Current_right() As Byte
        Dim Ret As Byte
        Dim Report(8) As Byte
        Report(0) = &H2S ' ReportID IIC write request
        Report(1) = &HC2S ' 2 bytes with IIC Start and Stop
        Report(2) = &HB0S ' MD23 address byte = ADR 0, write
        'Report(3) = &HCS
        Report(3) = CURRENT_MOTOR1


        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' swallow ACK report
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

        

        'System.Threading.Thread.Sleep(40)

        Report(0) = &H3S ' ReportID IIC read request
        Report(1) = &H2S ' 1 Bytes
        Report(2) = &HB1S        ' read address
        Report(3) = &HCS
        Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' read answer containing voltage value
        Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' extract bytes of value
        Current_right = Report(2)

    End Function
    Public Sub Reset_encoder()
        Dim Ret As Byte
        Dim Report(8) As Byte
        If Form2.IOWarrior <> 0 And (Form2.Pid = IOWKIT_PID_IOW40 Or Form2.Pid = IOWKIT_PID_IOW24) Then

            Report(0) = &H2 ' ReportID IIC write request
            Report(1) = &HC3 ' 2 bytes with IIC Start and Stop
            Report(2) = &HB0S ' MD23 address byte = ADR 0, write
            Report(3) = COMMAND
            Report(4) = RESET

            Ret = IowKitWrite(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
            ' swallow ACK report
            Ret = IowKitRead(Form2.IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        End If
    End Sub
End Module
