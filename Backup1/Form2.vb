Option Strict Off
Option Explicit Off
Public Class Form2

    Inherits System.Windows.Forms.Form
    Dim move_state As String
    Dim drive_angle As Single
    Dim start_ticks As Long
    Public IOWarrior As Integer
    Dim x_robot, y_robot, x_alt, y_alt As Integer
    Dim xd0, xd1, xd2, xd3, xd4, xd5, xd6 As Integer
    Dim yd0, yd1, yd2, yd3, yd4, yd5, yd6 As Integer
    Dim map(2000, 2000) As Integer
    Dim Grid As Form

    Public Pid As Integer



    Private Sub Button1_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button1.MouseClick
        forward()
        move_state = "forward"
    End Sub

    Private Sub Button2_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button2.MouseClick
        backward()
        move_state = "backward"
    End Sub

    Private Sub Button3_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button3.MouseClick
        turn_right()
        move_state = "right"
    End Sub

    Private Sub Button4_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button4.MouseClick
        turn_left()
        move_state = "left"
    End Sub

    Private Sub Button5_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Button5.MouseClick
        motor_stop()
        move_state = "stop"
    End Sub

    Private Sub Form2_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        Dim I As Integer
        Dim Report(8) As Byte

        ' deactivate SPI
        If IOWarrior <> 0 And (Pid = IOWKIT_PID_IOW40 Or Pid = IOWKIT_PID_IOW24) Then
            Report(0) = &H1S
            Report(1) = &H0S
            I = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        End If
        ' close IO-Warriors
        IowKitCloseDevice(IOWarrior)
    End Sub

    Private Sub Form2_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim I As Integer
        Dim Report(8) As Byte
        Dim Ret As Integer
        Grid = New Form
        Grid.Size = New Size(600, 600)
        Grid.Show()
        Dim z As System.Drawing.Graphics
        z = Me.CreateGraphics()
        Dim stift As New Pen(Color.Red, 2)
        Dim pinsel As New SolidBrush(Color.Red)

        ' open the IO-Warriors and use the first one available
        IOWarrior = IowKitOpenDevice
        Pid = IowKitGetProductId(IOWarrior)
        IowKitSetWriteTimeout(IOWarrior, 1000)
        ' if found activate IIC
        If IOWarrior <> 0 And (Pid = IOWKIT_PID_IOW40 Or Pid = IOWKIT_PID_IOW24) Then
            Report(0) = &H1S
            Report(1) = &H1S
            I = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

        End If
        If IOWarrior <> 0 And (Pid = IOWKIT_PID_IOW40 Or Pid = IOWKIT_PID_IOW24) Then
            ' Disable Timeout 1. Try
            Report(0) = &H2 ' ReportID IIC write request
            Report(1) = &HC3 ' 2 bytes with IIC Start and Stop
            Report(2) = &HB0S ' MD23 address byte = ADR 0, write
            Report(3) = COMMAND
            Report(4) = DISABLE_TIMEOUT

            Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
            ' swallow ACK report
            Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

            ' Disable Timeout 2. Try

            Report(0) = &H2 ' ReportID IIC write request
            Report(1) = &HC3 ' 2 bytes with IIC Start and Stop
            Report(2) = &HB0S
            ' MD23 address byte = ADR 0, write
            Report(3) = COMMAND
            Report(4) = DISABLE_TIMEOUT

            Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
            ' swallow ACK report
            Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

            ' Stop Motors
            'Left
            Report(0) = &H2 ' ReportID IIC write request
            Report(1) = &HC3 ' 2 bytes with IIC Start and Stop
            Report(2) = &HB0S ' MD23 address byte = ADR 0, write
            Report(3) = SPEED1
            Report(4) = 128
            Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
            ' swallow ACK report
            Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
            'Right
            Report(0) = &H2 ' ReportID IIC write request
            Report(1) = &HC3 ' 2 bytes with IIC Start and Stop
            Report(2) = &HB0S ' MD23 address byte = ADR 0, write
            Report(3) = SPEED1
            Report(4) = 128
            Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
            ' swallow ACK report
            Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
            Timer2.Interval = 1000
            Timer2.Enabled = False
            Timer1.Interval = 200
            Timer1.Enabled = True
            x_alt = 200
            y_alt = 200
            drive_angle = 0
        End If
    End Sub


    Private Sub Timer1_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Timer1.Tick
        Dim Distance(8) As Single
        Static Dim ticks, ticks_alt, ticks_neu As Single
        Dim z As System.Drawing.Graphics
        Dim Coordinate(2) As Integer

        z = Me.CreateGraphics()


        Dim stift, stift2 As New Pen(Color.Red, 3)

        Dim pinsel, pinsel2 As New SolidBrush(Color.Red)
        Distance = get_ir_sensors()

        Label5.Text = Distance(0)
        Label6.Text = Distance(1)
        Label7.Text = Distance(2)
        Label8.Text = Distance(3)
        Label9.Text = Distance(4)
        Label48.Text = Distance(5)
        Label49.Text = Distance(6)
        'Label50.Text = Distance(4)


        'Label18.Text = Revision()
        'Label13.Text = VB6.Format((Motor_Ticks_Left()))
        z.Clear(Me.BackColor)

        z.FillRectangle(pinsel, 100, 305 - 35, Distance(0) * 2, 15)
        z.FillRectangle(pinsel, 100, 330 - 35, Distance(1) * 2, 15)
        z.FillRectangle(pinsel, 100, 355 - 35, Distance(2) * 2, 15)
        z.FillRectangle(pinsel, 100, 380 - 35, Distance(3) * 2, 15)
        z.FillRectangle(pinsel, 100, 405 - 35, Distance(4) * 2, 15)
        z.FillRectangle(pinsel, 100, 430 - 35, Distance(5) * 2, 15)
        z.FillRectangle(pinsel, 100, 455 - 35, Distance(6) * 2, 15)
        z.Dispose()

        'avoid_collision(Distance)
        If (CheckBox1.Checked) Then sail(Distance)
        If (CheckBox2.Checked) Then matrix(Distance)
        Label11.Text = Voltage()
        If IsNothing(ticks_alt) Then ticks_alt = 0
        ticks_neu = (Motor_Ticks_Left() + Motor_Ticks_Right()) / 2
        ticks = ticks_neu - ticks_alt
        Label13.Text = ticks
        ' alle 100mm x und y neu berechen
        'If ticks > (start_ticks + 115) Then

        '        y = y_alt + (ticks / 115) * Math.Sin((drive_angle) * Math.PI / 180)
        '       x = x_alt + (ticks / 115) * Math.Cos((drive_angle) * Math.PI / 180)

        ' 1 gridpunkt sollte erstmal 1 cm sein, 1 tick = 0,87mm, 1cm ca 12 Ticks

        y_robot = y_alt + (ticks / 12) * Math.Cos((drive_angle) * Math.PI / 180)
        x_robot = x_alt + (ticks / 12) * Math.Sin((drive_angle) * Math.PI / 180)
        Generate_grid(Distance, x_robot, y_robot)

        Label53.Text = x_robot
        Label54.Text = y_robot
        y_alt = y_robot
        x_alt = x_robot
        ticks_alt = ticks_neu
        If (CheckBox3.Checked) Then check_current()
        Label31.Text = Current_left()
        Label3.Text = get_sonar()
    End Sub
    Private Sub Generate_grid(ByVal Dist() As Single, ByVal x As Integer, ByVal y As Integer)
        Dim z2 As System.Drawing.Graphics
        z2 = Grid.CreateGraphics()
        Dim stift2 As New Pen(Color.Red, 3)
        Dim stift3 As New Pen(Color.Black, 3)
        Dim i As Integer
        Dim sensor_angle() As Integer = {0, 45, 71, 90, 109, 135, 180}
        Dim xd, yd As Integer

        z2.Clear(Grid.BackColor)
        For i = 0 To 6
            If Dist(i) < 80 Then
                xd = Math.Cos(sensor_angle(i) * Math.PI / 180) * Dist(i)
                yd = Math.Sin(sensor_angle(i) * Math.PI / 180) * Dist(i)
                Coordinate = rotate_xy(xd, yd, drive_angle)
                z2.DrawEllipse(stift2, Coordinate(0) + x, 600 - (Coordinate(1) + y), 2, 2)
            End If
        Next
        z2.DrawEllipse(stift3, x, 600 - y, 2, 2)
        z2.Dispose()
    End Sub


    Private Function Revision() As Byte
        Dim Ret As Byte
        Dim Report(8) As Byte
        Report(0) = &H2S ' ReportID IIC write request
        Report(1) = &HC2S ' 2 bytes with IIC Start and Stop
        Report(2) = &HB0S ' MD23 address byte = ADR 0, write
        'Report(3) = &HCS
        Report(3) = &HDS


        Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' swallow ACK report
        Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

        Label20.Text = VB6.Format((Report(0)))
        Label21.Text = VB6.Format((Report(1)))
        Label22.Text = VB6.Format((Report(2)))

        'System.Threading.Thread.Sleep(40)

        Report(0) = &H3S ' ReportID IIC read request
        Report(1) = &H2S ' 1 Bytes
        Report(2) = &HB1S        ' read address
        Report(3) = &HCS
        Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' read answer containing voltage value
        Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' extract bytes of value
        Revision = Report(2)

    End Function
    Private Sub Timer2_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Timer2.Tick
        'Label18.Text = Revision()
        Label11.Text = Voltage()
        'Label13.Text = Motor_Ticks_Left()
        Label31.Text = Current_left()
        Label32.Text = Current_right()
    End Sub

    Private Sub forward()
        Dim left, right As Integer
        start_ticks = Motor_Ticks_Left()
        left = 160 'speed1
        right = 160 'speed2
        motor_drive(left, right)

    End Sub

    Private Sub backward()
        Dim left, right As Integer
        left = 86 'speed1
        right = 86 'speed2
        motor_drive(left, right)
    End Sub
    Private Sub turn_left()
        Dim left, right As Integer
        start_ticks = Motor_Ticks_Left()
        left = 118 'speed1
        right = 138 'speed2
        motor_drive(left, right)
    End Sub
    Private Sub turn_right()
        Dim left, right As Integer
        start_ticks = Motor_Ticks_Left()
        left = 138 'speed1
        right = 118 'speed2
        motor_drive(left, right)
    End Sub
    Private Sub motor_stop()
        Dim left, right As Integer
        left = 128 'speed1
        right = 128 'speed2
        motor_drive(left, right)
        'CheckBox1.Checked = False
        'CheckBox2.Checked = False
        If move_state = "left" Or move_state = "right" Then
            drive_angle = drive_angle + ((Motor_Ticks_Left() - start_ticks) / 2.5)
            Label42.Text = drive_angle
        End If
    End Sub
    Private Sub curve_left()
        Dim left, right As Integer
        left = 140 'speed1
        right = 160 'speed2
        motor_drive(left, right)
    End Sub
    Private Sub curve_right()
        Dim left, right As Integer
        left = 160 'speed1
        right = 140 'speed2
        motor_drive(left, right)
    End Sub
    Private Sub avoid_collision(ByVal PSD() As Single)
        'statt einer Wendung sollten die PSD-Werte mit den Gschwindigkeiten multipliziert werden
        'dazu müssen wir aber a) die PSD-Werte auf metrische normalisieren  
        ' und b) die Geschwindigkeit normalisieren

        Dim ticks As Long
        Const TICKS_90 As Integer = 100
        Const DISTANCE As Integer = 35
        If PSD(2) < DISTANCE Then  ' Mitte
            motor_brake()
            motor_stop()
            ticks = Motor_Ticks_Left()
            ticks = ticks + TICKS_90 ' 90 Grad nach Links
            curve_left()

            'turn_left()
            Do Until (ticks > Motor_Ticks_Left())
            Loop
            'motor_stop()
            forward()

            Return
        End If
        If PSD(1) < DISTANCE Then  ' vorne rechts
            motor_stop()
            ticks = Motor_Ticks_Left()
            ticks = ticks + TICKS_90 ' 90 Grad nach Links
            curve_left()

            'turn_left()
            Do Until (ticks > Motor_Ticks_Left())
            Loop
            'motor_stop()
            forward()

            Return
        End If
        If PSD(3) < DISTANCE Then  ' vorne links
            motor_stop()
            ticks = Motor_Ticks_Left()
            ticks = ticks + TICKS_90 ' 90 Grad
            curve_right()

            'turn_right()
            Do Until (ticks < Motor_Ticks_Left())
            Loop
            'motor_stop()
            forward()

            Return
        End If
    End Sub
    Private Sub motor_brake()
        Dim Ret As Integer
        Dim I As Integer
        Dim Report(8) As Byte

        If IOWarrior <> 0 And (Pid = IOWKIT_PID_IOW40 Or Pid = IOWKIT_PID_IOW24) Then
            For I = 10 To 0
                Report(0) = &H2 ' ReportID IIC write request
                Report(1) = &HC3 ' 3 bytes with IIC Start and Stop
                Report(2) = MD23_ADDRESS ' MD23 address byte = ADR 0, write
                Report(3) = SPEED1
                Report(4) = 128 + I
                Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
                ' swallow ACK report
                Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

                Report(0) = &H2 ' ReportID IIC write request
                Report(1) = &HC3 ' 3 bytes with IIC Start and Stop
                Report(2) = MD23_ADDRESS ' MD23 address byte = ADR 0, write
                Report(3) = SPEED2
                Report(4) = 128 + I
                Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
                ' swallow ACK report
                Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
            Next
        End If
    End Sub
    Private Sub motor_drive(ByVal left As Integer, ByVal right As Integer)
        Dim Ret As Integer

        Dim Report(8) As Byte

        If IOWarrior <> 0 And (Pid = IOWKIT_PID_IOW40 Or Pid = IOWKIT_PID_IOW24) Then

            Report(0) = &H2 ' ReportID IIC write request
            Report(1) = &HC3 ' 2 bytes with IIC Start and Stop
            Report(2) = &HB0S ' MD23 address byte = ADR 0, write
            Report(3) = SPEED1
            Report(4) = left
            Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
            ' swallow ACK report
            Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

            Report(0) = &H2 ' ReportID IIC write request
            Report(1) = &HC3 ' 2 bytes with IIC Start and Stop
            Report(2) = &HB0S ' MD23 address byte = ADR 0, write
            Report(3) = SPEED2
            Report(4) = right
            Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
            ' swallow ACK report
            Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        End If
    End Sub
    Private Sub drive_distance(ByVal distance As Integer)
        Dim left, right As Integer
        Const wheeldiameter As Double = 100
        Const pi As Double = 3.1415
        Const circum As Double = wheeldiameter * pi
        Const mmm_per_tick As Double = circum / 360
        Dim ticks As Double = distance / mmm_per_tick
        Dim a, b As Long
        a = Motor_Ticks_Left()
        Label36.Text = a
        Label38.Text = ticks
        b = a + ticks - 100 ' Bremsweg
        forward()
        Do Until (b < Motor_Ticks_Left())
            Label34.Text = Motor_Ticks_Left()
        Loop
        motor_stop()
        'Korrektur mit Regelung

    End Sub
    Private Sub drive_distance_back(ByVal distance As Integer)
        '1 Tick = 0,87 mm  100mm sind ca 115 ticks
        Dim left, right As Integer
        Const wheeldiameter As Double = 100
        Const pi As Double = 3.1415
        Const circum As Double = wheeldiameter * pi
        Const mmm_per_tick As Double = circum / 360
        Dim ticks As Double = distance / mmm_per_tick
        Dim a, b As Long
        a = Motor_Ticks_Left()
        Label36.Text = a
        Label38.Text = ticks
        b = a - ticks + 90 ' Bremsweg
        backward()
        Do Until (b > Motor_Ticks_Left())
            Label34.Text = Motor_Ticks_Left()
        Loop
        motor_stop()
        'Korrektur mit Regelung

    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Dim distance As Integer
        distance = CInt(TextBox1.Text)
        drive_distance(distance)
    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub Grad_links_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Grad_links.Click
        drive_angle = drive_angle + turn_degree_left(CInt(TextBox2.Text))
        Label42.Text = drive_angle
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        drive_angle = drive_angle + turn_degree_right(CInt(TextBox2.Text))
        Label42.Text = drive_angle
    End Sub

    Private Sub Label37_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label37.Click


    End Sub
    Private Function turn_degree_left(ByVal degree As Integer) As Single
        Dim a, b As Long
        Dim ticks As Integer
        Const umfang As Single = 785.4
        a = Motor_Ticks_Left()

        ticks = 2.5 * degree
        b = a - ticks + 20
        'disable_closed_loop()
        turn_left()
        Do Until (b > Motor_Ticks_Left())
            Label41.Text = Motor_Ticks_Left()
        Loop
        motor_stop()

        Label43.Text = (Motor_Ticks_Left() - a)
        'Label43.Text = System.Math.Abs(Motor_Ticks_Left() - a)
        'Label42.Text = (System.Math.Abs(Motor_Ticks_Left() - a)) / 2.5
        'enable_closed_loop()
        turn_degree_left = (Motor_Ticks_Left() - a) / 2.5
    End Function
    Private Function turn_degree_right(ByVal degree As Integer) As Single
        Dim a, b As Long
        Dim ticks As Integer
        Const umfang As Single = 785.4
        a = Motor_Ticks_Left()
        Label42.Text = a
        ticks = 2.5 * degree
        b = a + ticks - 20
        'disable_closed_loop()
        turn_right()
        Do Until (b < Motor_Ticks_Left())
            Label41.Text = Motor_Ticks_Left()
        Loop
        motor_stop()
        Label43.Text = (Motor_Ticks_Left() - a)
        'Label43.Text = System.Math.Abs(Motor_Ticks_Left() - a)
        'Label42.Text = (System.Math.Abs(Motor_Ticks_Left() - a)) / 2.5
        'enable_closed_loop()
        turn_degree_right = (Motor_Ticks_Left() - a) / 2.5
    End Function
    Private Sub sail(ByVal PSD() As Single)
        Dim left, right As Integer
        left = 140 'speed1
        right = 140 'speed2
        motor_drive(left, right)
        Const DISTANCE As Integer = 35
        If PSD(2) < DISTANCE Then  ' Mitte
            'slow
            motor_drive(left, right)
            right = right + 45 - (PSD(2))
            motor_drive(left, right)
            Do
                PSD = get_ir_sensors()
            Loop Until (PSD(2) > DISTANCE)
            motor_drive(140, 140)
            Return
        End If
        If PSD(1) < DISTANCE Then  ' vorne rechts
            'slow
            motor_drive(left, right)
            right = right + 45 - (PSD(1))
            motor_drive(left, right)
            Do
                PSD = get_ir_sensors()
            Loop Until (PSD(1) > DISTANCE)
            motor_drive(140, 140)
            Return
        End If
        If PSD(3) < DISTANCE Then  ' vorne links
            'slow
            motor_drive(left, right)
            left = left + 45 - (PSD(3))
            motor_drive(left, right)
            Do
                PSD = get_ir_sensors()
            Loop Until (PSD(3) > DISTANCE)
            motor_drive(140, 140)
            Return
        End If
    End Sub
    Private Sub matrix(ByVal PSD() As Single)

        Dim k As Integer
        Dim sensor_array(32) As String
        '                                L V/L M V/R R
        sensor_array(&H0) = "forward"   '0  0  0  0  0   nix
        sensor_array(&H1) = "forward"   '0  0  0  0  1   nix
        sensor_array(&H2) = "forleft"   '0  0  0  1  0   links 45
        sensor_array(&H3) = "forleft"   '0  0  0  1  1   links 45
        sensor_array(&H4) = "forleft"   '0  0  1  0  0   links 45 oder rechts 45
        sensor_array(&H5) = "forleft"   '0  0  1  0  1   links 45
        sensor_array(&H6) = "forleft"   '0  0  1  1  0   links 45
        sensor_array(&H7) = "forleft"   '0  0  1  1  1   links 45
        sensor_array(&H8) = "foright"   '0  1  0  0  0   rechts 45
        sensor_array(&H9) = "left"      '0  1  0  0  1   links 90
        sensor_array(&HA) = "left"      '0  1  0  1  0   links 90 oder recht 90
        sensor_array(&HB) = "left"      '0  1  0  1  1   links 90
        sensor_array(&HC) = "left"      '0  1  1  0  0   links 90 oder rechts 90
        sensor_array(&HD) = "left"      '0  1  1  0  1   links 90 
        sensor_array(&HE) = "left"      '0  1  1  1  0   links 90 oder rechts 90
        sensor_array(&HF) = "left"      '0  1  1  1  1   links 90
        sensor_array(&H10) = "forward"  '1  0  0  0  0   nix
        sensor_array(&H11) = "forward"  '1  0  0  0  1   nix
        sensor_array(&H12) = "foright"  '1  0  0  1  0   rechts 45
        sensor_array(&H13) = "forleft"  '1  0  0  1  1   links 45
        sensor_array(&H14) = "foright"  '1  0  1  0  0   rechts 45
        sensor_array(&H15) = "turn"     '1  0  1  0  1   rückwärts oder links 180 
        sensor_array(&H16) = "right"    '1  0  1  1  0   rechts 90
        sensor_array(&H17) = "turn"     '1  0  1  1  1   rückwärts oder links 180
        sensor_array(&H18) = "foright"  '1  1  0  0  0   rechts 45
        sensor_array(&H19) = "foright"  '1  1  0  0  1   rechts 45
        sensor_array(&H1A) = "right"    '1  1  0  1  0   rechts 90
        sensor_array(&H1B) = "turn"     '1  1  0  1  1   ??rückwärts oder links 180
        sensor_array(&H1C) = "foright"  '1  1  1  0  0   rechts 45
        sensor_array(&H1D) = "turn"     '1  1  1  0  1   rückwärts oder links 180
        sensor_array(&H1E) = "turn"     '1  1  1  1  0   rückwärts oder links 180
        sensor_array(&H1F) = "turn"     '1  1  1  1  1   rückwärts 
        k = 0
        Dim sensor As Byte
        For k = 4 To 0 Step -1
            sensor <<= 1
            If PSD(k) < 35 Then sensor = sensor Or 1

        Next
        Label45.Text = sensor_array(sensor)
        Select Case sensor_array(sensor)
            Case "left"
                turn_degree_left(210)
            Case "right"
                turn_degree_right(210)
            Case "foright"
                turn_degree_right(105) ' 45 Grad
            Case "forleft"
                turn_degree_left(105)
            Case "turn"
                turn_degree_left(420)
            Case Else
                ' nix
        End Select
        Select Case move_state
            Case "forward"
                forward()
            Case "backward"
                backward()
            Case "stop"
                motor_stop()


        End Select
    End Sub
    Private Function get_ir_sensors() As Single()
        Dim I, K As Integer
        Dim Ret As Integer
        Dim N(8, 4) As Single
        Dim Distance(8) As Single
        Dim Report(8) As Byte
        Const RESOLUTION As Single = 0.001220703125
        If IOWarrior <> 0 And (Pid = IOWKIT_PID_IOW40 Or Pid = IOWKIT_PID_IOW24) Then
            For K = 0 To 1
                'K = 0
                For I = 0 To 7
                    Report(0) = &H2S ' ReportID IIC write request
                    Report(1) = &HC2S ' 2 bytes with IIC Start and Stop
                    Report(2) = &H5ES ' MAXIM127 address byte = ADR 0, write
                    ' MAXIM127 control byte
                    ' START, I = Channel index, +/-10 V = RNG 1 and BIP 1, normal operation
                    Report(3) = &H80S Or (I * 16)
                    Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
                    ' swallow ACK report
                    Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

                    Report(0) = &H3S ' ReportID IIC write request
                    Report(1) = &H2S ' 2 bytes with IIC Start and Stop
                    Report(2) = &H5FS ' MAXIM127 address byte = ADR 0, read
                    ' MAXIM127 control byte
                    ' START, I = Channel index, +/-10 V = RNG 1 and BIP 1, normal operation
                    Report(3) = &H80S Or (I * 16)
                    Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
                    ' read answer containing voltage value
                    Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

                    ' extract bytes of value
                    N(I, K) = Report(2) And &HFFS
                    N(I, K) = N(I, K) * 256
                    N(I, K) = N(I, K) Or Report(3)
                    'N(I) = N(I) / 50
                    ' value is upper 12 bits 16 bits
                    N(I, K) = N(I, K) / 16
                    N(I, K) = N(I, K) * RESOLUTION
                    N(I, K) = (33 / N(I, K)) - 2  'Entfernung in cm

                Next
            Next

        End If

        For I = 0 To 7
            'Distance(I) = N(I, 0)
            'Distance(I) = (N(I, 0) + N(I, 1) + N(I, 2) + N(I, 3)) / 4
            Distance(I) = (N(I, 0) + N(I, 1)) / 2
            If Distance(I) > 80 Then Distance(I) = 80
        Next

        Return Distance
    End Function

    Private Sub CheckBox2_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox2.CheckedChanged

    End Sub

    Private Sub Label44_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label44.Click

    End Sub

    Private Sub check_current()
        Static Dim currents(4) As Byte
        Static Dim i As Byte
        Dim actual_current As Single
        Dim history_current As Single
        '------------------------------------
        'If IsNothing(i) Then
        'i = 0
        'currents(3) = Current_left()
        'currents(2) = currents(3)
        'currents(1) = currents(2)
        'currents(0) = currents(1)
        'End If

        'Label23.Text = currents(3)
        'Label24.Text = currents(2)
        'Label25.Text = currents(1)
        'Label28.Text = currents(0)
        'actual_current = Current_left()
        'history_current = (currents(3) + currents(2) + currents(1) + currents(0)) / 4
        'Label46.Text = history_current
        'If ((actual_current) > history_current) Then motor_stop()
        'currents(0) = currents(1)
        'currents(1) = currents(2)
        'currents(2) = currents(3)
        'currents(3) = actual_current
        '------------------------------
        actual_current = Current_left()
        If actual_current > 2 Then
            'Timer1.Enabled = False
            motor_stop()
            drive_distance_back(100)
            motor_stop()

            turn_degree_left(210)
            'Timer1.Enabled = True
        End If


    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        Dim distance As Integer
        distance = CInt(TextBox1.Text)
        drive_distance_back(distance)
    End Sub
    Private Function get_sonar() As Integer
        Dim Ret As Byte
        Dim Report(8) As Byte
        Report(0) = &H2S ' ReportID IIC write request
        Report(1) = &HC3S ' 2 bytes with IIC Start and Stop
        Report(2) = &HE0S ' MD23 address byte = ADR 0, write
        'Report(3) = &HCS
        Report(3) = &H0S
        Report(4) = &H51

        Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' swallow ACK report
        Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

        'Label20.Text = VB6.Format((Report(0)))
        System.Threading.Thread.Sleep(70)

        Report(0) = &H2S ' ReportID IIC write request
        Report(1) = &HC2S ' 2 bytes with IIC Start and Stop
        Report(2) = &HE0S ' 
        Report(3) = &H2S

        Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' swallow ACK report
        Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)

        Report(0) = &H3S ' ReportID IIC read request
        Report(1) = &H2S ' 1 Bytes
        Report(2) = &HE1S        ' read address

        Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' read answer containing voltage value
        Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        ' extract bytes of value
        get_sonar = Report(3) + (256 * Report(2))
        'Label3.Text = get_sonar
    End Function
    Private Function disable_closed_loop() As Integer
        Dim Ret As Byte
        Dim Report(8) As Byte
        If IOWarrior <> 0 And (Pid = IOWKIT_PID_IOW40 Or Pid = IOWKIT_PID_IOW24) Then

            Report(0) = &H2 ' ReportID IIC write request
            Report(1) = &HC3 ' 2 bytes with IIC Start and Stop
            Report(2) = &HB0S ' MD23 address byte = ADR 0, write
            Report(3) = COMMAND
            Report(4) = DISABLE_SPEED_REGULATION

            Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
            ' swallow ACK report
            Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        End If
    End Function
    Private Function enable_closed_loop() As Integer
        Dim Ret As Byte
        Dim Report(8) As Byte
        If IOWarrior <> 0 And (Pid = IOWKIT_PID_IOW40 Or Pid = IOWKIT_PID_IOW24) Then

            Report(0) = &H2 ' ReportID IIC write request
            Report(1) = &HC3 ' 2 bytes with IIC Start and Stop
            Report(2) = &HB0S ' MD23 address byte = ADR 0, write
            Report(3) = COMMAND
            Report(4) = ENABLE_SPEED_REGULATION

            Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
            ' swallow ACK report
            Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        End If
    End Function

    Private Sub Label3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub create_map()
        Static Dim map(2000, 2000) As Byte

    End Sub

    Private Sub Label52_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label52.Click

    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        Dim Ret As Byte
        Dim Report(8) As Byte
        If IOWarrior <> 0 And (Pid = IOWKIT_PID_IOW40 Or Pid = IOWKIT_PID_IOW24) Then

            Report(0) = &H2 ' ReportID IIC write request
            Report(1) = &HC3 ' 2 bytes with IIC Start and Stop
            Report(2) = &HB0S ' MD23 address byte = ADR 0, write
            Report(3) = COMMAND
            Report(4) = RESET

            Ret = IowKitWrite(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
            ' swallow ACK report
            Ret = IowKitRead(IOWarrior, IOW_PIPE_SPECIAL_MODE, Report(0), 8)
        End If
    End Sub
    Private Function rotate_xy(ByVal x As Integer, ByVal y As Integer, ByVal phi As Integer) As Object
        Dim coordinate(2) As Integer

        coordinate(0) = x * Math.Cos(phi * Math.PI / 180) + y * Math.Sin(phi * Math.PI / 180)
        coordinate(1) = y * Math.Cos(phi * Math.PI / 180) - x * Math.Sin(phi * Math.PI / 180)


        rotate_xy = coordinate
    End Function
End Class