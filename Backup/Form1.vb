Option Strict Off
Option Explicit On
Friend Class Form1
	Inherits System.Windows.Forms.Form
	Dim IOWarrior As Integer
	Dim Pid As Integer
	
	Private Sub Form1_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
		Dim I As Integer
		Dim Report(8) As Byte
		
		' open the IO-Warriors and use the first one available
		IOWarrior = IowKitOpenDevice
		Pid = IowKitGetProductId(IOWarrior)
		
		' if found activate IIC
		If IOWarrior <> 0 And (Pid = IOWKIT_PID_IOW40 Or Pid = IOWKIT_PID_IOW24) Then
			Report(0) = &H1s
			Report(1) = &H1s
			I = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
			' start timer for reading Voltages
			Timer1.Enabled = True
		End If
	End Sub
	Private Sub Form1_FormClosed(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        Dim I As Integer
		Dim Report(8) As Byte
		
		' stop timer
		Timer1.Enabled = False
		' deactivate SPI
		If IOWarrior <> 0 And (Pid = IOWKIT_PID_IOW40 Or Pid = IOWKIT_PID_IOW24) Then
			Report(0) = &H1s
			Report(1) = &H0s
            I = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
		End If
		' close IO-Warriors
        IowKitCloseDevice(IOWarrior)
	End Sub
	
	Private Sub Timer1_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Timer1.Tick
		Dim I As Integer
		Dim Ret As Integer
		Dim N As Integer
		Dim Report(8) As Byte
		
		If IOWarrior <> 0 And (Pid = IOWKIT_PID_IOW40 Or Pid = IOWKIT_PID_IOW24) Then
			For I = 0 To 7
				Report(0) = &H2s ' ReportID IIC write request
				Report(1) = &HC2s ' 2 bytes with IIC Start and Stop
				Report(2) = &H50s ' MAXIM127 address byte = ADR 0, write
				' MAXIM127 control byte
				' START, I = Channel index, +/-10 V = RNG 1 and BIP 1, normal operation
				Report(3) = &H8Cs Or (I * 16)
				Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
				' swallow ACK report
				Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
				
				Report(0) = &H3s ' ReportID IIC write request
				Report(1) = &H2s ' 2 bytes with IIC Start and Stop
				Report(2) = &H51s ' MAXIM127 address byte = ADR 0, read
				' MAXIM127 control byte
				' START, I = Channel index, +/-10 V = RNG 1 and BIP 1, normal operation
				Report(3) = &H8Cs Or (I * 16)
				Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
				' read answer containing voltage value
				Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
				
				' extract bytes of value
				N = Report(2) And &HFFs
				N = N * 256
				N = N Or Report(3)
				' value is upper 12 bits of 16 bits
				N = N / 16
				' value is 12 bit signed!
				' We need to add the upper twos complement sign bits for the Integer.
				' This is a sign extension from 12 bit to 32 bit.
				If (N And &H800s) <> 0 Then
					N = N Or &HFFFFF000
				End If
				' value now is a signed integer with 2047 = 10 V and -2047 = -10 V
				Values(I).Text = VB6.Format(N * 10# / 2048#, "0.00")
			Next 
		End If
	End Sub
End Class