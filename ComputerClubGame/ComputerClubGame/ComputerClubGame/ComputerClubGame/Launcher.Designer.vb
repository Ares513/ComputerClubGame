<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Launcher
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.StartButton = New System.Windows.Forms.Button()
        Me.ExitButton = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'StartButton
        '
        Me.StartButton.Location = New System.Drawing.Point(56, 156)
        Me.StartButton.Name = "StartButton"
        Me.StartButton.Size = New System.Drawing.Size(170, 33)
        Me.StartButton.TabIndex = 0
        Me.StartButton.Text = "Start"
        Me.StartButton.UseVisualStyleBackColor = True
        '
        'ExitButton
        '
        Me.ExitButton.Location = New System.Drawing.Point(56, 207)
        Me.ExitButton.Name = "ExitButton"
        Me.ExitButton.Size = New System.Drawing.Size(168, 37)
        Me.ExitButton.TabIndex = 1
        Me.ExitButton.Text = "Exit"
        Me.ExitButton.UseVisualStyleBackColor = True
        '
        'Launcher
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 262)
        Me.Controls.Add(Me.ExitButton)
        Me.Controls.Add(Me.StartButton)
        Me.Name = "Launcher"
        Me.Text = "Launcher"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents StartButton As System.Windows.Forms.Button
    Friend WithEvents ExitButton As System.Windows.Forms.Button
End Class
