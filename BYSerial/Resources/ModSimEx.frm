VERSION 4.00
Begin VB.Form Form1 
   Caption         =  "Form1"
   ClientHeight    =  3555
   ClientLeft      =  3195
   ClientTop       =  2565
   ClientWidth     =  5250
   Height          =  3555
   Left            =  3195
   LinkTopic       =   "Form1"
   ScaleHeight     =   3150
   ScaleWidth      =   5130
   Top             =  2565
   Width           =  5250
   Begin VB.TextBox Text2 
      Height          =  285
      Left            =  3360
      TabIndex        =   3
      Text            =   "Text2"
      Top             =  240
      Width           =  495
   End
   Begin VB.TextBox Text1 
      Height          =  285
      Left            =  3360
      TabIndex        =   2
      Text            =   "Text1"
      Top             =  600
      Width           =  855
   End
   Begin VB.Timer Timer1 
      Interval        =   1000
      Left            =  1200
      Top             =  1680
   End
   Begin VB.Label Label8 
      Caption         =  "Text1.text = temp"
      Height          =  255
      Left            =  1920
      TabIndex        =   9
      Top             =  2160
      Width           =  2655
   End
   Begin VB.Label Label7 
      Caption         =  "ModSim32.ReadPoint (1, 40100, temp)"
      Height          =  255
      Left            =  1920
      TabIndex        =   8
      Top             =  1920
      Width           =  2775
   End
   Begin VB.Label Label6 
      Caption         =  "tick = tick + 1"
      Height          =  255
      Left            =  1920
      TabIndex        =   7
      Top             =  1680
      Width           =  2775
   End
   Begin VB.Label Label5 
      Caption         =  "ModSim32.WritePoint (1, 40100, tick)"
      Height          =  255
      Left            =  1920
      TabIndex        =   6
      Top             =  1440
      Width           =  3015
   End
   Begin VB.Label Label4 
      Caption         =  "(Each Second)"
      Height          =  255
      Left            =  600
      TabIndex        =   5
      Top             =  1440
      Width           =  1095
   End
   Begin VB.Label Label3 
      Caption         =  "Timer Operation:"
      Height          =  255
      Left            =  480
      TabIndex        =   4
      Top             =  1200
      Width           =  1215
   End
   Begin VB.Label Label2 
      Caption         =  "Modbus Node Address"
      Height          =  255
      Left            =  1560
      TabIndex        =   1
      Top             =  240
      Width           =  1695
   End
   Begin VB.Label Label1 
      Caption         =  "40100"
      Height          =  255
      Left            =  2640
      TabIndex        =   0
      Top             =  600
      Width           =  615
   End
End
Attribute VB_Name = "Form1"
Attribute VB_Creatable = False
Attribute VB_Exposed = False
Public m_sim As IModSim

Dim tick As Integer
Dim device As Integer
Dim address As Long

Private Sub Form_Load()

Set m_sim = CreateObject("ModSim32.Document")

tick = 0
device = 1
address = 40100
Label1.Caption = address
Text2.Text = device

End Sub


Private Sub Timer1_Timer()
Dim status As Integer
Dim temp As Integer

status = m_sim.WritePoint(device, address, tick)
If status = 0 Then
    tick = tick + 1
    End If

status = m_sim.ReadPoint(device, address, temp)
If status = 0 Then
    Text1.Text = temp
    End If

End Sub


